using CardanoSharp.Wallet.Models.Transactions.Scripts;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
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

        public static byte[] Serialize(this ScriptAny scriptAny)
        {
            return scriptAny.GetCBOR().EncodeToBytes();
        }
    }
}
