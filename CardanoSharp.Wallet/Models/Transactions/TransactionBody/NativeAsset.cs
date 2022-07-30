using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class NativeAsset<T>
    {
        public NativeAsset()
        {
            Token = new Dictionary<byte[], T>();
        }

        public Dictionary<byte[], T> Token { get; set; }
    }
}
