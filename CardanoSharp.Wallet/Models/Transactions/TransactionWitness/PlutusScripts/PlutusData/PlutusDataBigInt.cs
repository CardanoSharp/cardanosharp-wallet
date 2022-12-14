using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.NativeScripts;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    // big_int = int / big_uint / big_nint
    // big_uint = #6.2(bounded_bytes)
    // big_nint = #6.3(bounded_bytes)

    // int
    public class PlutusDataInt : IPlutusData
    {
        public int Value { get; set; }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(CBORObject.FromObject(Value).EncodeToBytes());
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // big_uint = #6.2(bounded_bytes)
    public class PlutusDataUInt : IPlutusData
    {
        public byte[] Value { get; set; }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject(Value);
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // big_nint = #6.3(bounded_bytes)
    public class PlutusDataNInt : IPlutusData
    {
        public byte[] Value { get; set; }

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
        public static IPlutusData GetPlutusDataBigInt(this CBORObject dataCbor)
        {
            if (dataCbor == null)
            {
                throw new ArgumentNullException(nameof(dataCbor));
            }

            if (dataCbor.Type != CBORType.Integer)
            {
                throw new ArgumentException("dataCbor is not expected type CBORType.Integer");
            }

            var number = dataCbor.AsNumber();
            if (number.CanFitInInt32())
            {
                return dataCbor.GetPlutusDataInt();
            }
            else if (number.CanFitInInt64())
            {
                return dataCbor.GetPlutusDataUInt();
            }
            else
            {
                return dataCbor.GetPlutusDataNInt();
            }
        }

        public static PlutusDataInt GetPlutusDataInt(this CBORObject dataCbor)
        {
            if (dataCbor == null)
            {
                throw new ArgumentNullException(nameof(dataCbor));
            }

            if (dataCbor.Type != CBORType.Integer)
            {
                throw new ArgumentException("dataCbor is not expected type CBORType.Integer");
            }

            var number = dataCbor.AsNumber();
            if (!number.CanFitInInt32())
            {
                throw new ArgumentException("Attempting to deserialize dataCbor as int but number is larger than size int");
            }

            int data = (int)dataCbor.DecodeValueToInt32();
            PlutusDataInt plutusDataInt = new PlutusDataInt() { Value = data };
            return plutusDataInt;
        }

        public static PlutusDataUInt GetPlutusDataUInt(this CBORObject dataCbor)
        {
            if (dataCbor == null)
            {
                throw new ArgumentNullException(nameof(dataCbor));
            }

            if (dataCbor.Type != CBORType.Integer)
            {
                throw new ArgumentException("dataCbor is not expected type CBORType.Integer");
            }

            var number = dataCbor.AsNumber();
            if (!number.CanFitInInt64())
            {
                throw new ArgumentException("Attempting to deserialize dataCbor as uint but number is larger than size uint");
            }

            uint data = (uint)dataCbor.DecodeValueToInt64();
            byte[] byteArray = BitConverter.GetBytes(data);
            PlutusDataUInt plutusDataUInt = new PlutusDataUInt() { Value = byteArray };
            return plutusDataUInt;
        }

        public static PlutusDataNInt GetPlutusDataNInt(this CBORObject dataCbor)
        {
            if (dataCbor == null)
            {
                throw new ArgumentNullException(nameof(dataCbor));
            }

            if (dataCbor.Type != CBORType.Integer)
            {
                throw new ArgumentException("dataCbor is not expected type CBORType.Integer");
            }

            var number = dataCbor.AsNumber();
            if (!number.CanFitInUInt64())
            {
                throw new ArgumentException("Attempting to deserialize dataCbor as nint but number is larger than size nint");
            }

            uint data = (uint)dataCbor.DecodeValueToUInt64();
            byte[] byteArray = BitConverter.GetBytes(data);
            PlutusDataNInt plutusDataNInt = new PlutusDataNInt() { Value = byteArray };
            return plutusDataNInt;
        }
    }
}
