using System;
using System.Collections.Generic;
using System.Numerics;
using CardanoSharp.Wallet.Extensions;
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
            return CBORObject.FromObject(Value);
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // big_uint = #6.2(bounded_bytes)
    public class PlutusDataUInt : IPlutusData
    {
        public BigInteger Value { get; set; }

        public PlutusDataUInt(long number)
        {
            Value = new BigInteger(number);
        }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject((long)Value);
        }

        public byte[] Serialize()
        {
            return GetCBOR().EncodeToBytes();
        }
    }

    // big_nint = #6.3(bounded_bytes)
    public class PlutusDataNInt : IPlutusData
    {
        public BigInteger Value { get; set; }

        public PlutusDataNInt(long number)
        {
            Value = new BigInteger(number);
        }

        public CBORObject GetCBOR()
        {
            return CBORObject.FromObject((long)Value);
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
                return dataCbor.GetPlutusDataInt();

            if (number.IsNegative())
                return dataCbor.GetPlutusDataNInt();

            return dataCbor.GetPlutusDataUInt();
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
                throw new ArgumentException(
                    "Attempting to deserialize dataCbor as int but number is larger than size int"
                );
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
                throw new ArgumentException(
                    "Attempting to deserialize dataCbor as uint but number is larger than size uint"
                );
            }

            long data = (long)dataCbor.DecodeValueToInt64();
            PlutusDataUInt plutusDataUInt = new PlutusDataUInt(data);
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
            if (!number.IsNegative() || !number.CanFitInInt64())
            {
                throw new ArgumentException(
                    "Attempting to deserialize dataCbor as nint but number is not negative"
                );
            }

            long data = (long)dataCbor.DecodeValueToInt64();
            PlutusDataNInt plutusDataNInt = new PlutusDataNInt(data);
            return plutusDataNInt;
        }
    }
}
