using System.Collections.Generic;

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
        public PoolParams()
        {
            RewardAddress = new HashSet<byte[]>();
            PoolOwners = new HashSet<byte[]>();
            Relays = new HashSet<Relays>(); 
        }
        public byte[] Operator { get; set; }
        public byte[] VrfKeyhash { get; set; }
        public uint Pledge { get; set; }
        public Margin Cost { get; set; }
        public ICollection<byte[]> RewardAddress { get; set; }
        public ICollection<byte[]> PoolOwners { get; set; }
        public Relays Relays { get; set; }
        public PoolMetadata MyProperty { get; set; }
    }
}