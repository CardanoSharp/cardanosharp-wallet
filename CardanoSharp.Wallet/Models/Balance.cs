using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models
{
    public class Balance
    {
        public long Lovelaces { get; set; }
        public IList<Asset> Assets { get; set; }
    }
}