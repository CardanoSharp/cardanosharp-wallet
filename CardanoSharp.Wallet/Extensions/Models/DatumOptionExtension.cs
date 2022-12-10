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
            var cborDatum = CBORObject.NewArray();

            //datum_option = [ 0, $hash32 // 1, data ]
            if (datumOption.Hash is not null) {
                cborDatum.Add(0);
                cborDatum.Add(CBORObject.DecodeFromBytes(datumOption.Hash));
            }
            else if (datumOption.Data is not null) {
                cborDatum.Add(1);
                cborDatum.Add(CBORObject.FromObject(datumOption.Data.GetCBOR().EncodeToBytes()).WithTag(24));                
            }
            
            return cborDatum;
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
