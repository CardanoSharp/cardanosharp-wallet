namespace CardanoSharp.Wallet.Models.Transactions
{
    //pub enum CertificateKind
    //{
    //    StakeRegistration,
    //    StakeDeregistration,
    //    StakeDelegation,
    //    PoolRegistration,
    //    PoolRetirement,
    //    GenesisKeyDelegation,
    //    MoveInstantaneousRewardsCert,
    //}
    public partial class Certificate
    {
        public byte[] StakeRegistration { get; set; }
        public byte[] StakeDeregistration { get; set; }
        public StakeDelegation StakeDelegation { get; set; }
        public PoolRegistration PoolRegistration { get; set; }
        public PoolRetirement PoolRetirement { get; set; }
        public GenesisKeyDelegation GenesisKeyDelegation { get; set; }
        public MoveInstantaneousRewardsCert MoveInstantaneousRewardsCert { get; set; }
    }
}