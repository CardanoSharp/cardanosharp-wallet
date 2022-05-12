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
                    throw new NotImplementedException();
                    break;
                case CBORType.SimpleValue:
                    throw new NotImplementedException();
                    break;
                case CBORType.ByteString:
                    result = cborObject.ToString().Replace("h", "").Replace("'", "");
                    break;
                case CBORType.TextString:
                    throw new NotImplementedException();
                    break;
                case CBORType.Array:
                    throw new NotImplementedException();
                    break;
                case CBORType.Map:
                    throw new NotImplementedException();
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
