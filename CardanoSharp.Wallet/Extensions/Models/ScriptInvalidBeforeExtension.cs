using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptInvalidBeforeExtension
    {
        public static CBORObject GetCBOR(this ScriptInvalidBefore scriptInvalidBefore)
        {
            var scriptInvalidBeforeCbor = CBORObject.NewArray()
                .Add(4)
                .Add(scriptInvalidBefore.Before);

            return scriptInvalidBeforeCbor;
        }

        public static ScriptInvalidBefore GetScriptInvalidBefore(this CBORObject scriptInvalidBeforeCbor)
        {
            //validation
            if (scriptInvalidBeforeCbor == null)
            {
                throw new ArgumentException(nameof(scriptInvalidBeforeCbor));
            }
            if (scriptInvalidBeforeCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("scriptInvalidBeforeCbor is not expected type CBORType.Array");
            }
            if (scriptInvalidBeforeCbor.Values.Count != 2)
            {
                throw new ArgumentException("scriptInvalidBeforeCbor has unexpected number of elements (expected 2)");
            }

            //get data
            var scriptInvalidBefore = new ScriptInvalidBefore();
            scriptInvalidBefore.Before = scriptInvalidBeforeCbor[1].DecodeValueToUInt32();

            //return
            return scriptInvalidBefore;
        }

        public static byte[] Serialize(this ScriptInvalidBefore scriptInvalidBefore)
        {
            return scriptInvalidBefore.GetCBOR().EncodeToBytes();
        }

        public static ScriptInvalidBefore DeserializeScriptInvalidBefore(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetScriptInvalidBefore();
        }
    }
}
