using System;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class DatumOptionExtension
    {
        public static CBORObject GetCBOR(this DatumOption datumOption)
        {
            if (datumOption.Hash != null)
                return CBORObject.DecodeFromBytes(datumOption.Hash);

            if (datumOption.Data != null)
                return CBORObject.FromObject(datumOption.Data.GetCBOR().EncodeToBytes()).WithTag(24);
            
            return null;
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
