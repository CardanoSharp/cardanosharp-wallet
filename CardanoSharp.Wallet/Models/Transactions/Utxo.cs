using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public class Utxo
    {
        public string? TxHash { get; set; }
        public uint TxIndex { get; set; }
        public ulong Value { get; set; }
        public IEnumerable<Asset> AssetList { get; set; }
    }
}