using PeterO.Cbor2;
using System;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class CBORExtensions
    {
        public static object DecodeValueByCborType(this CBORObject cborObject)
        {
            object result = new object();

            switch (cborObject.Type)
            {
                case CBORType.Boolean:
                    result = cborObject.AsBoolean();
                    break;
                case CBORType.SimpleValue:
                    throw new NotImplementedException();
                case CBORType.ByteString:
                    result = cborObject.ToString().Replace("h", "").Replace("'", "");
                    break;
                case CBORType.TextString:
                    var raw = cborObject.ToString();
                    if (raw.Length > 1)
                    {
                        raw = raw.Substring(1);
                        raw = raw.Substring(0, raw.Length - 1);
                    }
                    result = raw;
                    break;
                case CBORType.Array:
                    var list = new List<object>();
                    foreach (var item in cborObject.Values)
                    {
                        list.Add(DecodeValueByCborType(item));
                    }
                    result = list.ToArray();
                    break;
                case CBORType.Map:
                    var map = new Dictionary<object, object>();
                    foreach (var key in cborObject.Keys)
                    {
                        var decodedKey = key.DecodeValueByCborType();
                        map[decodedKey] = DecodeValueByCborType(cborObject[key]);
                    }
                    result = map;
                    break;
                case CBORType.Integer:
                    var number = cborObject.AsNumber();
                    if (number.CanFitInInt32())
                    {
                        result = number.ToUInt32Checked();
                    }
                    else if (number.CanFitInInt64())
                    {
                        result = number.ToInt64Checked();
                    }
                    else
                    {
                        result = number.ToUInt64Unchecked();
                    }
                    break;
                case CBORType.FloatingPoint:
                    throw new NotImplementedException();
                default:
                    break;
            }

            return result;
        }

        public static short DecodeValueToInt16(this CBORObject cborObject)
        {
            if (cborObject.Type != CBORType.Integer)
                throw new ArgumentException("CBORObject must be of type integer", nameof(cborObject));
            var number = cborObject.AsNumber();
            return number.ToInt16Checked();
        }

        public static ushort DecodeValueToUInt16(this CBORObject cborObject)
        {
            if (cborObject.Type != CBORType.Integer)
                throw new ArgumentException("CBORObject must be of type integer", nameof(cborObject));
            var number = cborObject.AsNumber();
            return number.ToUInt16Checked();
        }
        public static int DecodeValueToInt32(this CBORObject cborObject)
        {
            if (cborObject.Type != CBORType.Integer)
                throw new ArgumentException("CBORObject must be of type integer", nameof(cborObject));
            var number = cborObject.AsNumber();
            return number.ToInt32Checked();
        }

        public static uint DecodeValueToUInt32(this CBORObject cborObject)
        {
            if (cborObject.Type != CBORType.Integer)
                throw new ArgumentException("CBORObject must be of type integer", nameof(cborObject));
            var number = cborObject.AsNumber();
            return number.ToUInt32Checked();
        }

        public static long DecodeValueToInt64(this CBORObject cborObject)
        {
            if (cborObject.Type != CBORType.Integer)
                throw new ArgumentException("CBORObject must be of type integer", nameof(cborObject));
            var number = cborObject.AsNumber();
            return number.ToInt64Checked();
        }

        public static ulong DecodeValueToUInt64(this CBORObject cborObject)
        {
            if (cborObject.Type != CBORType.Integer)
                throw new ArgumentException("CBORObject must be of type integer", nameof(cborObject));
            var number = cborObject.AsNumber();
            return number.ToUInt64Checked();
        }
    }
}
