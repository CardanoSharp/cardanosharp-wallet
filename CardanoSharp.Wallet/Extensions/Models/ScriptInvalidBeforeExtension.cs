using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class ScriptInvalidBeforeExtension
    {
        public static CBORObject GetCBOR(this ScriptInvalidBefore scriptInvalidBefore)
        {
            var scriptInvalidBeforeCbor = CBORObject.NewArray()
                .Add(4)
                .Add(scriptInvalidBefore.Before);

            return scriptInvalidBeforeCbor;
        }

        public static byte[] Serialize(this ScriptInvalidBefore scriptInvalidBefore)
        {
            return scriptInvalidBefore.GetCBOR().EncodeToBytes();
        }
    }
}
