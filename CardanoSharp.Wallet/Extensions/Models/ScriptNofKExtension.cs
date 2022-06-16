using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptNofKExtension
    {
        public static byte[] GetPolicyId(this ScriptNofK scriptNofK)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x00 });
            buffer.Write(scriptNofK.GetCBOR().EncodeToBytes());
            return HashUtility.Blake2b224(buffer.ToArray());
        }

        public static CBORObject GetCBOR(this ScriptNofK scriptNofK)
        {
            //script_n_of_k = (3, n: uint, [ * native_script ])
            var scriptNofKCbor = CBORObject.NewArray()
                .Add(3)
                .Add(scriptNofK.N);

            var nativeScriptCbor = CBORObject.NewArray();
            foreach (var nativeScript in scriptNofK.NativeScripts)
            {
                nativeScriptCbor.Add(nativeScript.GetCBOR2());
            }
            scriptNofKCbor.Add(nativeScriptCbor);

            return scriptNofKCbor;
        }

        public static ScriptNofK GetScriptNofK(this CBORObject scriptNofKCbor)
        {
            //validation
            if (scriptNofKCbor == null)
            {
                throw new ArgumentException(nameof(scriptNofKCbor));
            }
            if (scriptNofKCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("scriptNofKCbor is not expected type CBORType.Array");
            }
            if (scriptNofKCbor.Values.Count != 3)
            {
                throw new ArgumentException("scriptNofKCbor has unexpected number of elements (expected 3)");
            }

            //get data
            var scriptNofK = new ScriptNofK();
            scriptNofK.N = scriptNofKCbor[1].DecodeValueToUInt32();
            foreach (var nativeScriptCbor in scriptNofKCbor[2].Values)
            {
                scriptNofK.NativeScripts.Add(nativeScriptCbor.GetNativeScript());
            }

            //return
            return scriptNofK;
        }

        public static byte[] Serialize(this ScriptNofK scriptNofK)
        {
            return scriptNofK.GetCBOR().EncodeToBytes();
        }

        public static ScriptNofK DeserializeScriptNofK(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetScriptNofK();
        }
    }
}
