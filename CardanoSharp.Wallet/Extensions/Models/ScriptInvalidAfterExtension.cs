using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptInvalidAfterExtension
    {
        public static CBORObject GetCBOR(this ScriptInvalidAfter scriptInvalidAfter)
        {
            var scriptInvalidAfterCbor = CBORObject.NewArray()
                .Add(5)
                .Add(scriptInvalidAfter.After);

            return scriptInvalidAfterCbor;
        }

        public static ScriptInvalidAfter GetScriptInvalidAfter(this CBORObject scriptInvalidAfterCbor)
        {
            //validation
            if (scriptInvalidAfterCbor == null)
            {
                throw new ArgumentException(nameof(scriptInvalidAfterCbor));
            }
            if (scriptInvalidAfterCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("scriptInvalidAfterCbor is not expected type CBORType.Array");
            }
            if (scriptInvalidAfterCbor.Values.Count != 2)
            {
                throw new ArgumentException("scriptInvalidAfterCbor has unexpected number of elements (expected 2)");
            }

            //get data
            var scriptInvalidAfter = new ScriptInvalidAfter();
            scriptInvalidAfter.After = scriptInvalidAfterCbor[1].DecodeValueToUInt32();

            //return
            return scriptInvalidAfter;
        }

        public static byte[] Serialize(this ScriptInvalidAfter scriptInvalidAfter)
        {
            return scriptInvalidAfter.GetCBOR().EncodeToBytes();
        }

        public static ScriptInvalidAfter DeserializeScriptInvalidAfter(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetScriptInvalidAfter();
        }
    }
}
