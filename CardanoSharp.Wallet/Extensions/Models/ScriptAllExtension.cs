using CardanoSharp.Wallet.Models.Transactions.Scripts;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptAllExtension
    {
        public static CBORObject GetCBOR(this ScriptAll nativeScript)
        {
            //script_all = (1, [*native_script])

            var scriptAllCbor = CBORObject.NewArray()
                .Add(1)
                .Add()
        }

        public static byte[] Serialize(this ScriptAll nativeScript)
        {
            var scriptAllCbor = CBORObject.NewArray();



            return scriptAllCbor.EncodeToBytes();
        }
    }
}
