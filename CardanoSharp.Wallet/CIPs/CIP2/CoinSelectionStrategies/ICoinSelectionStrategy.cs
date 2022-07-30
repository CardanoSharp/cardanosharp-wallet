using System.Collections.Generic;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface ICoinSelectionStrategy
    {
        void SelectInputs(CoinSelection coinSelection, List<Utxo> utxos, long amount, Asset asset = null, int limit = 20);

        
    }
}