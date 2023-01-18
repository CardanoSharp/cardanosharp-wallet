using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions;

namespace CardanoSharp.Wallet.Models.Transactions
{
    //transaction_input = [transaction_id: $hash32
    //                    , index : uint
    //                    ]
    public partial class TransactionInput
    {
        public byte[] TransactionId { get; set; }
        public uint TransactionIndex { get; set; }
    }

    public class TransactionInputComparer : IComparer<TransactionInput>
    {
        public int Compare(TransactionInput x, TransactionInput y)
        {
            int txCompare = string.Compare(
                x.TransactionId.ToStringHex(),
                y.TransactionId.ToStringHex()
            );
            if (txCompare != 0)
                return txCompare;
            else
                return x.TransactionIndex.CompareTo(y.TransactionIndex);
        }
    }
}
