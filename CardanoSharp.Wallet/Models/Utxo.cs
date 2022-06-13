using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.Models
{
    public class Utxo
    {
        public string? TxHash { get; set; }
        public uint TxIndex { get; set; }
        public ulong Value { get; set; }
        public IEnumerable<Asset> AssetList { get; set; }
    }
}