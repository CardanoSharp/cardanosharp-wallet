using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class TransactionOutputValue
    {
        public ulong Coin { get; set; }
        /// <summary>
        /// BPlusTree<byte[], NativeAsset>
        /// byte[] = PolicyID
        /// </summary>
        public Dictionary<byte[], NativeAsset> MultiAsset { get; set; }
    }
}
