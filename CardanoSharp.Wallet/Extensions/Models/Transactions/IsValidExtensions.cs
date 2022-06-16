using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static class IsValidExtensions
    {
        public static CBORObject GetCBOR(this bool isValid)
        {
            return CBORObject.FromObject(isValid);
        }

        public static bool GetIsValid(this CBORObject isValidCbor)
        {
            //validation
            if (isValidCbor == null)
            {
                throw new ArgumentNullException(nameof(isValidCbor));
            }
            if (isValidCbor.Type != CBORType.Boolean)
            {
                throw new ArgumentException("isValidCbor is not expected type CBORType.Boolean");
            }

            //get data
            var isValid = isValidCbor.AsBoolean();

            //return
            return isValid;
        }

        public static byte[] Serialize(this bool isValid)
        {
            return isValid.GetCBOR().EncodeToBytes();
        }

        public static bool DeserializeIsValid(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetIsValid();
        }
    }
}
