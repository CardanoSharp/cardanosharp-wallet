using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using CardanoSharp.Wallet.Utilities;
using System;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class NativeScriptExtension
    {
        public static byte[] GetPolicyId(this NativeScript nativeScript)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x00 });
            buffer.Write(nativeScript.GetCBOR2().EncodeToBytes());
            return HashUtility.Blake2b224(buffer.ToArray());
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

        [Obsolete("Will be deprecated. Please use GetCBOR2() instead")]
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

        public static NativeScript GetNativeScript(this CBORObject nativeScriptCbor)
        {
            if (nativeScriptCbor == null)
            {
                throw new ArgumentException(nameof(nativeScriptCbor));
            }
            if (nativeScriptCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("nativeScriptCbor is not expected type CBORType.Array");
            }
            if (nativeScriptCbor.Values.Count < 2)
            {
                throw new ArgumentException("nativeScriptCbor has unexpected number of elements (expected 2+)");
            }

            var nativeScript = new NativeScript();
            var nativeScriptTypeIndex = nativeScriptCbor[0].DecodeValueToInt32();
            if (nativeScriptTypeIndex < 0 || nativeScriptTypeIndex > 5)
            {
                throw new ArgumentException("nativeScriptCbor first element (index) has value outside expected range (expected 0..5)");
            }

            //var nativeScriptKey = ((string)nativeScriptCbor[1].DecodeValueByCborType()).HexToByteArray();
            switch (nativeScriptTypeIndex)
            {
                case 0:
                    nativeScript.ScriptPubKey = nativeScriptCbor.GetScriptPubKey();
                    break;
                case 1:
                    nativeScript.ScriptAll = nativeScriptCbor.GetScriptAll();
                    break;
                case 2:
                    nativeScript.ScriptAny = nativeScriptCbor.GetScriptAny();
                    break;
                case 3:
                    nativeScript.ScriptNofK = nativeScriptCbor.GetScriptNofK();
                    break;
                case 4:
                    nativeScript.InvalidBefore = nativeScriptCbor.GetScriptInvalidBefore();
                    break;
                case 5:
                    nativeScript.InvalidAfter = nativeScriptCbor.GetScriptInvalidAfter();
                    break;
            }

            return nativeScript;
        }

        public static byte[] Serialize(this NativeScript nativeScript)
        {
            return nativeScript.GetCBOR2().EncodeToBytes();
        }

        public static NativeScript DeserializeNativeScript(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetNativeScript();
        }
    }
}
