using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class NativeAsset
    {
        public NativeAsset()
        {
            Token = new Dictionary<byte[], ulong>();
        }

        public Dictionary<byte[], ulong> Token { get; set; }
    }
}
