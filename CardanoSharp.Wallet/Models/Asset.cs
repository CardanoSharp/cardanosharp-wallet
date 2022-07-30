using System.Collections.Generic;

#nullable enable
namespace CardanoSharp.Wallet.Models
{
    public class Asset
    {
        public string PolicyId { get; set; }
        public string Name { get; set; }
        public long Quantity { get; set; }
    }
}