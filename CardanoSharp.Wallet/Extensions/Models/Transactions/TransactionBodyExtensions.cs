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

        public static byte[] Serialize(this TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return transactionBody.GetCBOR(auxiliaryData).EncodeToBytes();
        }
    }
}
