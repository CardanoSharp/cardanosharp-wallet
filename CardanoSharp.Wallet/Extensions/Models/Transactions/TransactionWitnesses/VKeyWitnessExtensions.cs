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
            
            //validation
            if (vKeyWitness.VKey.Key.Length != 32)
            {
                throw new ArgumentException("vKeyWitness.VKey.Key not expected length (expected 32)");
            }
            
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
            if (vKeyWitnessCbor == null)
            {
                throw new ArgumentNullException(nameof(vKeyWitnessCbor));
            }
            if (vKeyWitnessCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("vKeyWitnessCbor is not expected type CBORType.Array");
            }
            if (vKeyWitnessCbor.Count != 2)
            {
                throw new ArgumentException("vKeyWitnessCbor has unexpected number elements (expected 2)");
            }

            //get data
            var vkeyWitness = new VKeyWitness();
            var key = ((string)vKeyWitnessCbor[0].DecodeValueByCborType()).HexToByteArray();
            vkeyWitness.VKey = new PublicKey(key, null);
            vkeyWitness.Signature = ((string)vKeyWitnessCbor[1].DecodeValueByCborType()).HexToByteArray();

            //return
            return vkeyWitness;
        }

        public static byte[] Serialize(this VKeyWitness vKeyWitness, TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return vKeyWitness.GetCBOR(transactionBody, auxiliaryData).EncodeToBytes();
        }

        public static VKeyWitness DeserializeVKeyWitness(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetVKeyWitness();
        }
    }
}
