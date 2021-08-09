using CardanoSharp.Wallet.Models.Transactions.Scripts;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptPubKeyExtension
    {
        public static CBORObject GetCBOR(this ScriptPubKey scriptPubKey)
        {
            // script_pubkey = (0, addr_keyhash)
            var scriptPubKeyCbor = CBORObject.NewArray()
                .Add(0)
                .Add(scriptPubKey.KeyHash);

            return scriptPubKeyCbor;
        }

        public static byte[] Serialize(this ScriptPubKey scriptPubKey)
        {
            return scriptPubKey.GetCBOR().EncodeToBytes();
        }
    }
}
