using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class NativeScriptExtension
    {
        public static byte[] GetPolicyId(this NativeScript )
        {
            BigEndianBuffer buffer = new BigEndianBuffer();
            buffer.Write(new byte[] { 0x00 });
            buffer.Wr
        }
    }
}
