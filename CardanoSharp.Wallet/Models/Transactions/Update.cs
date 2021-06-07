namespace CardanoSharp.Wallet.Models.Transactions
{
    //update = [proposed_protocol_parameter_updates
    //     , epoch
    //     ]
    public partial class Update
    {
        public byte[] ProposedProtocolParameterUpdates { get; set; }
        public int Epoch { get; set; }
    }
}