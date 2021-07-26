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
            // invalid_hereafter = (5, uint)
            var scriptInvalidBeforeCbor = CBORObject.NewArray()
                .Add(5)
                .Add(scriptInvalidBefore.Before);

            return scriptInvalidBeforeCbor;
        }
    }
}
