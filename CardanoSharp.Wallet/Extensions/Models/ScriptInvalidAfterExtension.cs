using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptInvalidAfterExtension
    {
        public static CBORObject GetCBOR(this ScriptInvalidAfter scriptInvalidAfter)
        {
            // invalid_before = (4, uint)
            var scriptInvalidAfterCbor = CBORObject.NewArray()
                .Add(4)
                .Add(scriptInvalidAfter.After);

            return scriptInvalidAfterCbor;
        }

        public static byte[] Serialize(this ScriptInvalidAfter scriptInvalidAfter)
        {
            return scriptInvalidAfter.GetCBOR().EncodeToBytes();
        }
    }
}
