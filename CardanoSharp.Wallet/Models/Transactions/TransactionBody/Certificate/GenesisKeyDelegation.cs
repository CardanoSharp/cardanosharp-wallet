using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class GenesisKeyDelegation
    {
        public GenesisKeyDelegation()
        {
            GenesisHash = new HashSet<byte[]>(); 
        }
        public ICollection<byte[]> GenesisHash { get; set; }
        public byte[] GenesisDelegateHash { get; set; }
        public byte[] VRFKeyHash { get; set; }
    }
}