using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class MoveInstantaneousRewardsCert
    {
        public int MIRPot { get; set; }
        public Dictionary<byte[], uint> Rewards { get; set; }
    }
}