using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{

    //update = [proposed_protocol_parameter_updates
    //     , epoch
    //     ]
    public partial class Update
    {
        public Dictionary<byte[], object> ProposedProtocolParameterUpdates { get; set; }
        public uint Epoch { get; set; }

    }
}