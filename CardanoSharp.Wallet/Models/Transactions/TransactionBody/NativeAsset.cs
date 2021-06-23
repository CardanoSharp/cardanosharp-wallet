using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class NativeAsset
    {
        public Dictionary<byte[], uint> Token { get; set; }
    }
}
