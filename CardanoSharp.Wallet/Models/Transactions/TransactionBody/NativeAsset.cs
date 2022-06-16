using System.Collections.Generic;

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
