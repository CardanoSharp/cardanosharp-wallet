using CardanoSharp.Wallet.Models.Transactions.Scripts;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptAnyExtension
    {
        public static CBORObject GetCBOR(this ScriptAny scriptAny)
        {
            //script_any = (2, [ * native_script ])
            var scriptAnyCbor = CBORObject.NewArray()
                .Add(2);

            foreach (var nativeScript in scriptAny.NativeScripts)
            {
                scriptAnyCbor.Add(nativeScript.GetCBOR());
            }

            return scriptAnyCbor;
        }

        public static ScriptAny GetScriptAny(this CBORObject scriptAnyCbor)
        {
            //validation
            if (scriptAnyCbor == null)
            {
                throw new ArgumentNullException(nameof(scriptAnyCbor));
            }
            if (scriptAnyCbor.Count < 2)
            {
                throw new ArgumentException("scriptAllCbor has unexpected number of elements (expected 2+)");
            }

            //get data
            var scriptAny = new ScriptAny();
            foreach (var nativeScriptCbor in scriptAnyCbor.Values.Skip(1))
            {
                scriptAny.NativeScripts.Add(nativeScriptCbor.GetNativeScript());
            }
            return scriptAny;
        }

        public static byte[] Serialize(this ScriptAny scriptAny)
        {
            return scriptAny.GetCBOR().EncodeToBytes();
        }
    }
}
