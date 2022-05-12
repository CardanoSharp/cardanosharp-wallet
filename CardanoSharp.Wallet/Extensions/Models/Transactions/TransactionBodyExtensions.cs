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
                throw new NullReferenceException("Transaction body CBOR is null");
            }
            if (transactionBodyCbor.Type != CBORType.Map)
            {
                throw new InvalidOperationException("Transaction body CBOR is not Map type");
            }
            if (!transactionBodyCbor.ContainsKey(0))
            {
                throw new InvalidOperationException("Inputs key not present");
            }

            //get data
            var inputsCbor = transactionBodyCbor.Values.First();
            var outputsCbor = transactionBodyCbor.Values.Skip(1).First();
            var fee = Convert.ToUInt64(transactionBodyCbor.Values.Skip(2).First().DecodeValueByCborType());
            uint? ttl;
            if (true)
            {
                ttl = Convert.ToUInt32(transactionBodyCbor.Values.Skip(3).FirstOrDefault().DecodeValueByCborType());
            }

            //populate
            var transactionBody = new TransactionBody();
            foreach (var input in inputsCbor.Values)
            {
                var inputAddress = (string)input.Values.First().DecodeValueByCborType();
                var inputIndex = Convert.ToUInt32(input.Values.Skip(1).First().DecodeValueByCborType());
                transactionBody.TransactionInputs.Add(new TransactionInput()
                {
                    TransactionIndex = inputIndex,
                    TransactionId = inputAddress.ToBytes()
                });
            }
            foreach (var output in outputsCbor.Values)
            {
                var outputAddress = (string)output.Values.First().DecodeValueByCborType();
                var outputCoin = Convert.ToUInt64(output.Values.Skip(1).First().DecodeValueByCborType());
                transactionBody.TransactionOutputs.Add(new TransactionOutput()
                {
                    Address = outputAddress.ToBytes(),
                    Value = new TransactionOutputValue() { Coin = outputCoin }
                });
            }
            transactionBody.Fee = fee;
            transactionBody.Ttl = ttl;

            //return
            return transactionBody;
        }

        public static byte[] Serialize(this TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return transactionBody.GetCBOR(auxiliaryData).EncodeToBytes();
        }
    }
}
