using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class Update
    {
        public Dictionary<byte[], object> ProposedProtocolParameterUpdates { get; set; }
        public uint Epoch { get; set; }

    }
}