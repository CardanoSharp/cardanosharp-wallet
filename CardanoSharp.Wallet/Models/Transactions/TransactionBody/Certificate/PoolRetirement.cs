namespace CardanoSharp.Wallet.Models.Transactions
{
    //    pub struct PoolRetirement
    //    {
    //        pool_keyhash: Ed25519KeyHash,
    //    epoch: Epoch,
    //}
    public partial class PoolRetirement
    {
        public byte[] PoolKeyHash { get; set; }
        public uint Epoch { get; set; }
    }
}