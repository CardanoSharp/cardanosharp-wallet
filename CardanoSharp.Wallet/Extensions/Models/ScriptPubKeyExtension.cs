using CardanoSharp.Wallet.Models.Transactions.Scripts;
using PeterO.Cbor2;
using System;

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

        public static ScriptPubKey GetScriptPubKey(this CBORObject scriptPubKeyCbor)
        {
            //validation
            if (scriptPubKeyCbor == null)
            {
                throw new ArgumentNullException(nameof(scriptPubKeyCbor));
            }
            if (scriptPubKeyCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("scriptPubKeyCbor is not expected type CBORType.Array");
            }
            if (scriptPubKeyCbor.Values.Count != 2)
            {
                throw new ArgumentException("scriptPubKeyCbor has unexpected number of elements (expected 2)");
            }

            //get data
            var scriptPubKey = new ScriptPubKey();
            var nativeScriptKey = ((string)scriptPubKeyCbor[1].DecodeValueByCborType()).HexToByteArray();
            scriptPubKey.KeyHash = nativeScriptKey;

            //return
            return scriptPubKey;
        }

        public static byte[] Serialize(this ScriptPubKey scriptPubKey)
        {
            return scriptPubKey.GetCBOR().EncodeToBytes();
        }

        public static ScriptPubKey DeserializeScriptPubKey(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetScriptPubKey();
        }

    }
}
