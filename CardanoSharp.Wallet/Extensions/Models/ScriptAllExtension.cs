using CardanoSharp.Wallet.Models.Transactions.Scripts;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptAllExtension
    {
        public static byte[] Serialize(this ScriptAll nativeScript)
        {
            var scriptAllCbor = CBORObject.NewArray();



            return scriptAllCbor.EncodeToBytes();
        }
    }
}
