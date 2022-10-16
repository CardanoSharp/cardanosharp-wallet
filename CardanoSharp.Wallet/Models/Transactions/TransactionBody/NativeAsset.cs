using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class NativeAsset
    {
        public NativeAsset()
        {
            Token = new Dictionary<byte[], long>();
        }

        public Dictionary<byte[], long> Token { get; set; }
    }
}
