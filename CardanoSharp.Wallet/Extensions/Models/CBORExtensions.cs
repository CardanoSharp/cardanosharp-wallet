using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

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
                    break;
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
                        result = number.ToInt32Checked();
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
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
