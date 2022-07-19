using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.Models
{
    public class Utxo
    {
        public string? TxHash { get; set; }
        public uint TxIndex { get; set; }
        public Balance Balance { get; set; }
    }
}