using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static class TransactionInputExtensions
    {
        public static CBORObject GetCBOR(this TransactionInput transactionInput)
        {
            return CBORObject.NewArray()
                .Add(transactionInput.TransactionId)
                .Add(transactionInput.TransactionIndex);
        }

        public static byte[] Serialize(this TransactionInput transactionInput)
        {
            return transactionInput.GetCBOR().EncodeToBytes();
        }
    }
}
