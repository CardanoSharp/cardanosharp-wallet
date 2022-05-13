using CardanoSharp.Wallet.Extensions.Models.Certificates;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static class TransactionBodyExtensions
    {
        public static CBORObject GetCBOR(this TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            CBORObject cborInputs = null;
            CBORObject cborOutputs = null;
            CBORObject cborTransactionMint = null;
            CBORObject cborBody = CBORObject.NewMap();

            //add all the transaction inputs
            if (transactionBody.TransactionInputs.Any())
            {
                cborInputs = CBORObject.NewArray();
                foreach (var txInput in transactionBody.TransactionInputs)
                {
                    cborInputs.Add(txInput.GetCBOR());
                }
            }

            if (cborInputs != null) cborBody.Add(0, cborInputs);


            //add all the transaction outputs
            if (transactionBody.TransactionOutputs.Any())
            {
                cborOutputs = CBORObject.NewArray();
                foreach (var txOutput in transactionBody.TransactionOutputs)
                {
                    cborOutputs.Add(txOutput.GetCBOR());
                }
            }

            if (cborOutputs != null) cborBody.Add(1, cborOutputs);

            //add fee
            cborBody.Add(2, transactionBody.Fee);

            //add ttl
            if (transactionBody.Ttl.HasValue) cborBody.Add(3, transactionBody.Ttl.Value);

            //add certificates
            if (transactionBody.Certificate != null)
            {
                cborBody.Add(4, transactionBody.Certificate.GetCBOR());
            }

            //add metadata
            if (auxiliaryData != null)
            {
                cborBody.Add(7, HashUtility.Blake2b256(auxiliaryData.GetCBOR().EncodeToBytes()));
            }

            //add tokens for minting
            if(transactionBody.Mint.Any())
            {
                cborTransactionMint = CBORObject.NewMap();
                foreach (var nativeAsset in transactionBody.Mint)
                {
                    var assetCbor = CBORObject.NewMap();
                    foreach (var asset in nativeAsset.Value.Token)
                    {
                        assetCbor.Add(asset.Key, asset.Value);
                    }
                    cborTransactionMint.Add(nativeAsset.Key, assetCbor);
                }
                cborBody.Add(9, cborTransactionMint);
            }

            return cborBody;
        }

        public static TransactionBody GetTransactionBody(this CBORObject transactionBodyCbor)
        {
            //validation
            if (transactionBodyCbor == null)
            {
                throw new ArgumentNullException(nameof(transactionBodyCbor));
            }
            if (transactionBodyCbor.Type != CBORType.Map)
            {
                throw new ArgumentException("transactionBodyCbor is not expected type CBORType.Map");
            }
            if (!transactionBodyCbor.ContainsKey(0))
            {
                throw new ArgumentException("transactionBodyCbor key 0 (Inputs) not present");
            }
            if (!transactionBodyCbor.ContainsKey(1))
            {
                throw new ArgumentException("transactionBodyCbor key 1 (Outputs) not present");
            }
            if (!transactionBodyCbor.ContainsKey(2))
            {
                throw new ArgumentException("transactionBodyCbor key 2 (Fee/Coin) not present");
            }

            //get data
            var transactionBody = new TransactionBody();
            //0 : set<transaction_input>    ; inputs
            var inputsCbor = transactionBodyCbor[0];
            foreach (var input in inputsCbor.Values)
            {
                var inputAddress = ((string)input.Values.First().DecodeValueByCborType()).HexToByteArray();
                var inputIndex = Convert.ToUInt32(input.Values.Skip(1).First().DecodeValueByCborType());
                transactionBody.TransactionInputs.Add(new TransactionInput()
                {
                    TransactionIndex = inputIndex,
                    TransactionId = inputAddress
                });
            }

            //1 : [* transaction_output]
            var outputsCbor = transactionBodyCbor[1];
            foreach (var output in outputsCbor.Values)
            {
                var outputAddress = ((string)output.Values.First().DecodeValueByCborType()).HexToByteArray();
                var outputCoin = Convert.ToUInt64(output.Values.Skip(1).First().DecodeValueByCborType());
                transactionBody.TransactionOutputs.Add(new TransactionOutput()
                {
                    Address = outputAddress,
                    Value = new TransactionOutputValue() { Coin = outputCoin }
                });
            }

            //2 : coin                      ; fee
            transactionBody.Fee = Convert.ToUInt64(transactionBodyCbor[2].DecodeValueByCborType());

            //? 3 : uint                    ; time to live
            if (transactionBodyCbor.ContainsKey(3))
            {
                transactionBody.Ttl = Convert.ToUInt32(transactionBodyCbor[3].DecodeValueByCborType());
            }

            //? 4 : [* certificate]
            if (transactionBodyCbor.ContainsKey(4))
            {
                transactionBody.Certificate = transactionBodyCbor[4].GetCertificate();
            }

            //? 5 : withdrawals
            //? 6 : update
            //? 7 : auxiliary_data_hash
            if (transactionBodyCbor.ContainsKey(7))
            {
            }

            //? 8 : uint                    ; validity interval start
            //? 9 : mint
            if (transactionBodyCbor.ContainsKey(9))
            {
            }

            //? 11 : script_data_hash; New
            //? 13 : set<transaction_input>; Collateral; new
            //? 14 : required_signers; New
            //? 15 : network_id; New

            //return
            return transactionBody;
        }

        public static byte[] Serialize(this TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return transactionBody.GetCBOR(auxiliaryData).EncodeToBytes();
        }
    }
}
