using System;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class RedeemerExtensions
    {
        public static CBORObject GetCBOR(this Redeemer redeemer)
        {
            // redeemer = [ tag: redeemer_tag, index: uint, data: plutus_data, ex_units: ex_units ]
            var cborRedeemer = CBORObject.NewArray();
            cborRedeemer.Add((uint)redeemer.Tag);
            cborRedeemer.Add(redeemer.Index);
            cborRedeemer.Add(redeemer.PlutusData.GetCBOR());
            cborRedeemer.Add(redeemer.ExUnits.GetCBOR());            
            return cborRedeemer;
        }

        public static Redeemer GetRedeemer(this CBORObject redeemerCbor)
        {
            if (redeemerCbor == null)
            {
                throw new ArgumentNullException(nameof(redeemerCbor));
            }

            if (redeemerCbor.Type !=  CBORType.Array)
            {
                throw new ArgumentException("redeemerCbor is not expected type CBORType.Array");
            }

            if (redeemerCbor.Count != 4)
            {
                throw new ArgumentException("redeemerCbor has unexpected number of elements (expected 4)");
            }
            
            Redeemer redeemer = new Redeemer();
            redeemer.Tag = (RedeemerTag)redeemerCbor[0].DecodeValueToInt32();
            redeemer.Index = (uint)redeemerCbor[1].DecodeValueToInt32();
            redeemer.PlutusData = (IPlutusData)redeemerCbor[2].DecodeValueByCborType();
            redeemer.ExUnits = (ExUnits)redeemerCbor[3].GetExUnits();
            return redeemer;          
        }

        public static byte[] Serialize(this Redeemer redeemer)
        {
            return redeemer.GetCBOR().EncodeToBytes();
        }

        public static Redeemer Deserialize(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetRedeemer();
        }
    }
}
