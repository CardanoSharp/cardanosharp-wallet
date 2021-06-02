namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class Certificate
    {
        public byte[] StakeRegistration { get; set; }
        public byte[] StakeDeregistration { get; set; }
        public byte[] StakeDelegation { get; set; }
        public byte[] PoolRegistration { get; set; }
        public byte[] PoolRetirement { get; set; }
        public byte[] GenesisKeyDelegation { get; set; }
        public byte[] MoveInstantaneousRewardsCert { get; set; }
    }
}