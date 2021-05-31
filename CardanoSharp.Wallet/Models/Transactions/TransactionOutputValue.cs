using CSharpTest.Net.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class TransactionOutputValue
    {
        public uint Coin { get; set; }
        /// <summary>
        /// BPlusTree<byte[], NativeAsset>
        /// byte[] = PolicyID
        /// </summary>
        public BPlusTree<byte[], NativeAsset> MultiAsset { get; set; }
    }
}
