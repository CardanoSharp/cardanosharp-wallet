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
    }
}
