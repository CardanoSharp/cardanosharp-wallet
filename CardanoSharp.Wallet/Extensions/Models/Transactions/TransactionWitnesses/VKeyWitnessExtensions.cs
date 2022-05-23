using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Utilities;
using Chaos.NaCl;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions.TransactionWitnesses
{
    public static class VKeyWitnessExtensions
    {
        public static CBORObject GetCBOR(this VKeyWitness vKeyWitness, TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            //validation
            if (vKeyWitness.VKey.Key.Length != 32)
            {
                throw new ArgumentException("vKeyWitness.VKey.Key not expected length (expected 32)");
            }

            //sign body
            var txBodyHash = HashUtility.Blake2b256(transactionBody.GetCBOR(auxiliaryData).EncodeToBytes());
            vKeyWitness.Signature = vKeyWitness.SKey.Sign(txBodyHash);

            //fill out cbor structure for vkey witnesses
            return CBORObject.NewArray()
                .Add(vKeyWitness.VKey.Key)
                .Add(vKeyWitness.Signature);
        }

        public static byte[] Serialize(this VKeyWitness vKeyWitness, TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return vKeyWitness.GetCBOR(transactionBody, auxiliaryData).EncodeToBytes();
        }
    }
}
