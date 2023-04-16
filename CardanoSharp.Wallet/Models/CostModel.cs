using System.Collections.Generic;
using CardanoSharp.Wallet.Enums;

namespace CardanoSharp.Wallet.Models
{
    public class CostModel
    {
        public Language Language { get; set; }
        public long[] Costs { get; set; }
    }
}