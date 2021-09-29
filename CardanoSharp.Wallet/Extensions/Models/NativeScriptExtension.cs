using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using CardanoSharp.Wallet.Utilities;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class NativeScriptExtension
    {
        public static byte[] GetPolicyId(this NativeScript nativeScript)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x00 });
            buffer.Write(nativeScript.GetCBOR().EncodeToBytes());
            return HashUtility.Blake2b244(buffer.ToArray());
        }

        public static CBORObject GetCBOR2(this NativeScript nativeScript)
        {
            if(nativeScript.ScriptPubKey != null)
                return nativeScript.ScriptPubKey.GetCBOR();

            if (nativeScript.ScriptAll != null)
                return nativeScript.ScriptAll.GetCBOR();

            if (nativeScript.ScriptAny != null)
                return nativeScript.ScriptAny.GetCBOR();

            if (nativeScript.ScriptNofK != null)
                return nativeScript.ScriptNofK.GetCBOR();

            if (nativeScript.InvalidAfter != null)
                return nativeScript.InvalidAfter.GetCBOR();

            if (nativeScript.InvalidBefore != null)
                return nativeScript.InvalidBefore.GetCBOR();

            return null;
        }

        public static CBORObject GetCBOR(this NativeScript nativeScript)
        {
            var nativeScriptCbor = CBORObject.NewArray();

            if (nativeScript.ScriptPubKey != null)
                nativeScriptCbor.Add(nativeScript.ScriptPubKey.GetCBOR());

            if (nativeScript.ScriptAll != null)
                nativeScriptCbor.Add(nativeScript.ScriptAll.GetCBOR());

            if (nativeScript.ScriptAny != null)
                nativeScriptCbor.Add(nativeScript.ScriptAny.GetCBOR());

            if (nativeScript.ScriptNofK != null)
                nativeScriptCbor.Add(nativeScript.ScriptNofK.GetCBOR());

            if (nativeScript.InvalidAfter != null)
                nativeScriptCbor.Add(nativeScript.InvalidAfter.GetCBOR());

            if (nativeScript.InvalidBefore != null)
                nativeScriptCbor.Add(nativeScript.InvalidBefore.GetCBOR());

            return nativeScriptCbor;
        }

        public static byte[] Serialize(this NativeScript nativeScript)
        {
            return nativeScript.GetCBOR().EncodeToBytes();
        }
    }
}
