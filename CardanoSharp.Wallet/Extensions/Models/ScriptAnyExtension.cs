using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptAnyExtension
    {
        public static byte[] GetPolicyId(this ScriptAny scriptAny)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x00 });
            buffer.Write(scriptAny.GetCBOR().EncodeToBytes());
            return HashUtility.Blake2b224(buffer.ToArray());
        }

        public static CBORObject GetCBOR(this ScriptAny scriptAny)
        {
            //script_any = (2, [ * native_script ])
            var scriptAnyCbor = CBORObject.NewArray()
                .Add(2);

            var nativeScriptCbor = CBORObject.NewArray();
            foreach (var nativeScript in scriptAny.NativeScripts)
            {
                nativeScriptCbor.Add(nativeScript.GetCBOR2());
            }
            scriptAnyCbor.Add(nativeScriptCbor);

            return scriptAnyCbor;
        }

        public static ScriptAny GetScriptAny(this CBORObject scriptAnyCbor)
        {
            //validation
            if (scriptAnyCbor == null)
            {
                throw new ArgumentNullException(nameof(scriptAnyCbor));
            }
            if (scriptAnyCbor.Count != 2)
            {
                throw new ArgumentException("scriptAllCbor has unexpected number of elements (expected 2)");
            }

            //get data
            var scriptAny = new ScriptAny();
            foreach (var nativeScriptCbor in scriptAnyCbor[1].Values)
            {
                scriptAny.NativeScripts.Add(nativeScriptCbor.GetNativeScript());
            }
            return scriptAny;
        }

        public static byte[] Serialize(this ScriptAny scriptAny)
        {
            return scriptAny.GetCBOR().EncodeToBytes();
        }

        public static ScriptAny DeserializeScriptAny(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetScriptAny();
        }
    }
}
