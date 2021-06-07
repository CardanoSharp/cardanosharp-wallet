using CardanoSharp.Wallet.Common;

namespace CardanoSharp.Wallet.Models.Transactions
{

//    pub struct PoolParams
//    {
//    operator: Ed25519KeyHash,
//    vrf_keyhash: VRFKeyHash,
//    pledge: Coin,
//    cost: Coin,
//    margin: UnitInterval,
//    reward_account: RewardAddress,
//    pool_owners: Ed25519KeyHashes,
//    relays: Relays,
//    pool_metadata: Option<PoolMetadata>,
//}
    
    //TODO Finish out this domain object
    public partial class PoolParams
    {
        public byte[] Operator { get; set; }
        public byte[] VrfKeyhash { get; set; }
        public uint Pledge { get; set; }
        public Margin Cost { get; set; }
    }
}