using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface ICoinSelectionStrategy
    {
        public List<Utxo> SelectInputs(List<Utxo> utxos, ulong amount, Asset asset = null);
        public List<TransactionOutput> CreateChange(List<Utxo> utxos, ulong amount, Asset asset = null);
    }
}