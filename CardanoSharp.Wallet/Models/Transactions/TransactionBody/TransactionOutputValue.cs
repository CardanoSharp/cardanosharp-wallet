using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class TransactionOutputValue
    {
        public ulong Coin { get; set; }
        /// <summary>
        /// BPlusTree<byte[], NativeAsset>
        /// byte[] = PolicyID
        /// </summary>
        public Dictionary<byte[], NativeAsset<ulong>> MultiAsset { get; set; }
    }
}
