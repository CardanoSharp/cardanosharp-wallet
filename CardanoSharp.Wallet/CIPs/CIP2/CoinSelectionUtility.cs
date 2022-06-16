using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public static class CoinSelectionUtility
    {
        public static CoinSelection UseLargestFirst(this TransactionBodyBuilder tbb, List<Utxo> utxos)
        {
            var cs = new CoinSelectionService(new LargestFirstStrategy(), null);
            var tb = tbb.Build();
            return cs.GetCoinSelection(tb.TransactionOutputs.ToList(), utxos);
        }

        public static CoinSelection UseRandomImprove(this TransactionBodyBuilder tbb, List<Utxo> utxos)
        {
            var cs = new CoinSelectionService(new RandomImproveStrategy(), new SingleTokenBundleStrategy());
            var tb = tbb.Build();
            return cs.GetCoinSelection(tb.TransactionOutputs.ToList(), utxos);
        }

        public static CoinSelection UseLargestFirstWithImprove(this TransactionBodyBuilder tbb, List<Utxo> utxos)
        {
            var cs = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
            var tb = tbb.Build();
            return cs.GetCoinSelection(tb.TransactionOutputs.ToList(), utxos);
        }
    }
}