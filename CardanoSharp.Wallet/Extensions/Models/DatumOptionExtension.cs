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
            if (datumOption.Hash is not null)
            {
                cborDatum.Add(0);
                cborDatum.Add(CBORObject.DecodeFromBytes(datumOption.Hash));
            }
            else if (datumOption.Data is not null)
            {
                cborDatum.Add(1);
                cborDatum.Add(datumOption.Data.GetCBOR().WithTag(24));
            }

            return cborDatum;
        }

        public static DatumOption GetDatumOption(this CBORObject datumOptionCbor)
        {
            if (datumOptionCbor == null)
            {
                throw new ArgumentNullException(nameof(datumOptionCbor));
            }

            if (datumOptionCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("datumOptionCbor is not expected type CBORType.Array");
            }

            if (datumOptionCbor.Count != 2)
            {
                throw new ArgumentException(
                    "datumOptionCbor has unexpected number of elements (expected 2)"
                );
            }

            DatumOption datumOption = new DatumOption();
            var datumType = datumOptionCbor[0].DecodeValueToInt32();
            if (datumType == 0)
            {
                datumOption.Hash = (byte[])datumOptionCbor[1].DecodeValueByCborType();
            }
            else if (datumType == 1)
            {
                var dataCbor = datumOptionCbor[1].Untag();
                datumOption.Data = dataCbor.GetPlutusData();
            }

            return datumOption;
        }

        public static string? Datum(this DatumOption datumOption)
        {
            if (datumOption.Data == null)
                return null;

            return (string)datumOption.Data.GetCBOR().DecodeValueByCborType();
        }

        public static byte[] Serialize(this DatumOption datumOption)
        {
            return datumOption.GetCBOR().EncodeToBytes();
        }

        public static DatumOption Deserialize(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetDatumOption();
        }
    }
}
