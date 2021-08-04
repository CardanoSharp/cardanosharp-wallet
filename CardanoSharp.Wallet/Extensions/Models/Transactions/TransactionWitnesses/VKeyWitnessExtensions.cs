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
            //sign body
            var txBodyHash = HashUtility.Blake2b256(transactionBody.GetCBOR(auxiliaryData).EncodeToBytes());
            if (vKeyWitness.SKey.Length == 32)
            {
                vKeyWitness.SKey = Ed25519.ExpandedPrivateKeyFromSeed(vKeyWitness.SKey.Slice(0, 32));
                vKeyWitness.Signature = Ed25519.Sign(txBodyHash, vKeyWitness.SKey);
            }
            else
            {
                vKeyWitness.Signature = Ed25519.SignCrypto(txBodyHash, vKeyWitness.SKey);
            }

            //fill out cbor structure for vkey witnesses
            return CBORObject.NewArray()
                .Add(vKeyWitness.VKey)
                .Add(vKeyWitness.Signature);
        }

        public static byte[] Serialize(this VKeyWitness vKeyWitness, TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return vKeyWitness.GetCBOR(transactionBody, auxiliaryData).EncodeToBytes();
        }
    }
}
