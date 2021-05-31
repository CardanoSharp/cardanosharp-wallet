using CSharpTest.Net.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class NativeAsset
    {
        public BPlusTree<byte[], uint> Token { get; set; }
    }
}
