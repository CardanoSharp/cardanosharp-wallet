using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptAllExtension
    {
        public static byte[] GetPolicyId(this ScriptAll scriptAll)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x00 });
            buffer.Write(scriptAll.GetCBOR().EncodeToBytes());
            return HashUtility.Blake2b224(buffer.ToArray());
        }

        public static CBORObject GetCBOR(this ScriptAll scriptAll)
        {
            //script_all = (1, [*native_script])
            var scriptAllCbor = CBORObject.NewArray()
                .Add(1);

            var nativeScriptCbor = CBORObject.NewArray();
            foreach (var nativeScript in scriptAll.NativeScripts)
            {
                nativeScriptCbor.Add(nativeScript.GetCBOR2());
            }
            scriptAllCbor.Add(nativeScriptCbor);

            return scriptAllCbor;
        }

        public static ScriptAll GetScriptAll(this CBORObject scriptAllCbor)
        {
            //validation
            if (scriptAllCbor == null)
            {
                throw new ArgumentNullException(nameof(scriptAllCbor));
            }
            if (scriptAllCbor.Count != 2)
            {
                throw new ArgumentException("scriptAllCbor has unexpected number of elements (expected 2)");
            }

            //get data
            var scriptAll = new ScriptAll();
            foreach (var nativeScriptCbor in scriptAllCbor[1].Values)
            {
                scriptAll.NativeScripts.Add(nativeScriptCbor.GetNativeScript());
            }

            //return
            return scriptAll;
        }

        public static byte[] Serialize(this ScriptAll scriptAll)
        {
            return scriptAll.GetCBOR().EncodeToBytes();
        }

        public static ScriptAll DeserializeScriptAll(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetScriptAll();
        }
    }
}
