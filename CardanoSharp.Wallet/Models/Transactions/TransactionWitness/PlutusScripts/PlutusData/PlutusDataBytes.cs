using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    // bounded_bytes
    public class PlutusDataBytes : IPlutusData
    {
        public byte[] Value { get; set; }

        public PlutusDataBytes() { }

        public PlutusDataBytes(byte[] bytes)
        {
            Value = bytes;
        }

        public void SetHex(string hexString)
        {
            Value = hexString.HexToByteArray();
        }

        public void SetString(string normalString)
        {
            Value = normalString.ToBytes();
        }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(Value);
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            PlutusDataBytes other = (PlutusDataBytes)obj;
            return Value.SequenceEqual(other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (byte b in Value)
                {
                    hash = hash * 31 + b;
                }
                return hash;
            }
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

            PlutusDataBytes plutusDataBytes = new PlutusDataBytes(dataCbor.GetByteString());
            return plutusDataBytes;
        }
    }
}
