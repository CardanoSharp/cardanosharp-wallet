using System;
using CardanoSharp.Wallet.Extensions;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    // bounded_bytes
    public class PlutusDataBytes : IPlutusData
    {
        private byte[] Value { get; set; }

        public PlutusDataBytes(CBORObject cbor)
        {
            Value = cbor.EncodeToBytes();
        }

        public PlutusDataBytes(string value)
        {
            Value = CBORObject.FromObject(value.ToBytes()).EncodeToBytes();
        }

        public CBORObject GetCBOR()
        {
            return CBORObject.DecodeFromBytes(Value);
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    public static partial class PlutusDataExtensions
    {
        public static PlutusDataBytes GetPlutusDataBytes(this CBORObject dataCbor)
        {
            if (dataCbor == null)
            {
                throw new ArgumentNullException(nameof(dataCbor));
            }

            if (dataCbor.Type != CBORType.ByteString)
            {
                throw new ArgumentException("dataCbor is not expected type CBORType.ByteString");
            }

            PlutusDataBytes plutusDataBytes = new PlutusDataBytes(dataCbor);
            return plutusDataBytes;
        }
    }
}
