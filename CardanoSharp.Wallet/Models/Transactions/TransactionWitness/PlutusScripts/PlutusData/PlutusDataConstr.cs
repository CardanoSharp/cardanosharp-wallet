using System;
using CardanoSharp.Wallet.CIPs.CIP30.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    //constr<plutus_data>
    //     constr<a> =
    //     #6.121([* a])
    //   / #6.122([* a])
    //   / #6.123([* a])
    //   / #6.124([* a])
    //   / #6.125([* a])
    //   / #6.126([* a])
    //   / #6.127([* a])
    //   ; similarly for tag range: 6.1280 .. 6.1400 inclusive
    //   / #6.102([uint, [* a]])
    public class PlutusDataConstr : IPlutusData
    {
        public static readonly long GENERAL_FORM_TAG = 102;
        public long Alternative { get; set; } = 0;
        public PlutusDataArray Value { get; set; }

        public CBORObject GetCBOR()
        {
            long? cborTag = alternativeToCompactCborTag(Alternative)!;

            CBORObject cbor;
            if (cborTag != null)
            {
                cbor = Value.GetCBOR().WithTag(cborTag);
            }
            else
            {
                var cborArray = CBORObject.NewArray();
                cborArray.Add(Alternative);
                cborArray.Add(Value.GetCBOR());
                cbor = cborArray;
                cbor.WithTag(GENERAL_FORM_TAG);
            }

            return cbor;
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }

        public static long? alternativeToCompactCborTag(long alt)
        {
            if (alt <= 6)
            {
                return 121 + alt;
            }
            else if (alt >= 7 && alt <= 127)
            {
                return 1280 - 7 + alt;
            }
            else
                return null;
        }

        public static long? compactCborTagToAlternative(long cborTag)
        {
            if (cborTag >= 121 && cborTag <= 127)
            {
                return cborTag - 121;
            }
            else if (cborTag >= 1280 && cborTag <= 1400)
            {
                return cborTag - 1280 + 7;
            }
            else
                return null;
        }
    }

    public static partial class PlutusDataExtensions
    {
        public static PlutusDataConstr GetPlutusDataConstr(this CBORObject dataCbor)
        {
            if (dataCbor == null)
            {
                throw new ArgumentNullException(nameof(dataCbor));
            }

            if (dataCbor.Type != CBORType.Array)
            {
                throw new ArgumentException(
                    "dataCbor is not expected type CBORType.Array (with constr tag)"
                );
            }

            long alternative;
            PlutusDataArray plutusDataArray;
            if ((long)dataCbor.MostOuterTag == PlutusDataConstr.GENERAL_FORM_TAG)
            {
                var untaggedDataCbor = dataCbor.Untag();
                if (untaggedDataCbor.Count != 2)
                {
                    throw new ArgumentException(
                        "dataCbor has unexpected number of elements for tag 102 (expected 2)"
                    );
                }

                alternative = untaggedDataCbor[0].DecodeValueToInt64();
                plutusDataArray = untaggedDataCbor[1].GetPlutusDataArray();
            }
            else
            {
                var untaggedDataCbor = dataCbor.Untag();
                long tag = (long)dataCbor.MostOuterTag;

                alternative = (long)PlutusDataConstr.compactCborTagToAlternative(tag)!;
                plutusDataArray = untaggedDataCbor.GetPlutusDataArray();
            }

            return new PlutusDataConstr() { Alternative = alternative, Value = plutusDataArray };
        }
    }
}
