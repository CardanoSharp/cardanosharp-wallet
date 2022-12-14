
using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    // bounded_bytes
    public class PlutusDataBytes : IPlutusData
    {
        public byte[] Value { get; set; }

        public void SetValue(string value)
        {
            Value = value.ToBytes();
        }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(Value);
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

            PlutusDataBytes plutusDataBytes = new PlutusDataBytes() { Value = dataCbor.EncodeToBytes() };
            return plutusDataBytes;
        }
    }
}
