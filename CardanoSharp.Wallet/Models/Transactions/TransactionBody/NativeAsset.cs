using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class NativeAsset
    {
        public NativeAsset()
        {
            Token = new Dictionary<byte[], uint>();
        }

        public Dictionary<byte[], uint> Token { get; set; }
    }
}
