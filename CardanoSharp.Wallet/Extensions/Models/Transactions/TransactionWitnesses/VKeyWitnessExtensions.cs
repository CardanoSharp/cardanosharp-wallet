using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Utilities;
using Chaos.NaCl;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions.TransactionWitnesses
{
    public static class VKeyWitnessExtensions
    {
        public static CBORObject GetCBOR(this VKeyWitness vKeyWitness, TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            //only do signing if skey is present (wont be present for deserialized transactions but signature will already be there)
            if (vKeyWitness.SKey != null)
            {
                //sign body
                var txBodyHash = HashUtility.Blake2b256(transactionBody.GetCBOR(auxiliaryData).EncodeToBytes());
                vKeyWitness.Signature = vKeyWitness.SKey.Sign(txBodyHash);
            }

            //fill out cbor structure for vkey witnesses
            return CBORObject.NewArray()
                .Add(vKeyWitness.VKey.Key)
                .Add(vKeyWitness.Signature);
        }

        public static VKeyWitness GetVKeyWitness(this CBORObject vKeyWitnessCbor)
        {
            //validation

            //get data
            var key = ((string)vKeyWitnessCbor.Values.First().DecodeValueByCborType()).HexToByteArray();
            var vKey = new PublicKey(key, null);
            vKey.Key = key;
            var signature = ((string)vKeyWitnessCbor.Values.Skip(1).First().DecodeValueByCborType()).HexToByteArray();

            //populate
            var vkeyWitness = new VKeyWitness()
            {
                VKey = vKey,
                Signature = signature
            };

            //return
            return vkeyWitness;
        }

        public static byte[] Serialize(this VKeyWitness vKeyWitness, TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return vKeyWitness.GetCBOR(transactionBody, auxiliaryData).EncodeToBytes();
        }
    }
}
