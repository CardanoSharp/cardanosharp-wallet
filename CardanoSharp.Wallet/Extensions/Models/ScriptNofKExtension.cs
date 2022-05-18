using CardanoSharp.Wallet.Models.Transactions.Scripts;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptNofKExtension
    {
        public static CBORObject GetCBOR(this ScriptNofK scriptNofK)
        {
            //script_n_of_k = (3, n: uint, [ * native_script ])
            var scriptNofKCbor = CBORObject.NewArray()
                .Add(3)
                .Add(scriptNofK.N);

            foreach (var nativeScript in scriptNofK.NativeScripts)
            {
                scriptNofKCbor.Add(nativeScript.GetCBOR());
            }

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
            if (scriptNofKCbor.Values.Count != 2)
            {
                throw new ArgumentException("scriptNofKCbor has unexpected number of elements (expected 2)");
            }

            //get data
            var scriptNofK = new ScriptNofK();
            scriptNofK.N = (uint)scriptNofKCbor[1].DecodeValueByCborType();

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
