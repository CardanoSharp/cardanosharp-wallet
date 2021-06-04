namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class Certificate
    {
        public byte[] StakeRegistration { get; set; }
        public byte[] StakeDeregistration { get; set; }
        public StakeDelegation StakeDelegation { get; set; }
        public PoolRegistration PoolRegistration { get; set; }
        public PoolRetirement PoolRetirement { get; set; } //TODO Finish Type
        public byte[] GenesisKeyDelegation { get; set; }//TODO Finish Type
        public byte[] MoveInstantaneousRewardsCert { get; set; }//TODO Finish Type
    }
}