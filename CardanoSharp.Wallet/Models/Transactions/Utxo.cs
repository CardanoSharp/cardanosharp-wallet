using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public class Utxo
    {
        public string TxHash { get; set; }
        public uint TxIndex { get; set; }
        public string OutputAddress { get; set; }
        public ulong Value { get; set; }
        public IEnumerable<Asset> AssetList { get; set; }
    }
}