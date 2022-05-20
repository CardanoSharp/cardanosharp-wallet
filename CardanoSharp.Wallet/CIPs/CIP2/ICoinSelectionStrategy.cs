using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface ICoinSelectionStrategy
    {
        public List<TransactionUnspentOutput> SelectInputs(List<TransactionUnspentOutput> utxos, ulong amount, Asset asset = null);
    }
}