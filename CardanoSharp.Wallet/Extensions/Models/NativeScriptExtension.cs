using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class NativeScriptExtension
    {
        public static byte[] GetPolicyId(this NativeScript nativeScript)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x00 });
            buffer.Write(nativeScript.GetCBOR().EncodeToBytes());
            return HashHelper.Blake2b244(buffer.ToArray());
        }

        public static CBORObject GetCBOR(this NativeScript nativeScript)
        {
            var nativeScriptCbor = CBORObject.NewArray();

            if (nativeScript.ScriptAll != null)
            {
                nativeScriptCbor.Add(nativeScript.ScriptAll.GetCBOR());
            }
        }
    }
}
