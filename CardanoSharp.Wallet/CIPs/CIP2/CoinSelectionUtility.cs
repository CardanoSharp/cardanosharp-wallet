using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.TransactionBuilding;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public static class CoinSelectionUtility
    {
        public static CoinSelection UseLargestFirst(this TransactionBodyBuilder tbb, List<Utxo> utxos, string changeAddress, ITokenBundleBuilder mint = null, int limit = 20, ulong feeBuffer = 0)
        {
            var cs = new CoinSelectionService(new LargestFirstStrategy(), new BasicChangeSelectionStrategy());
            var tb = tbb.Build();
            return cs.GetCoinSelection(tb.TransactionOutputs.ToList(), utxos, changeAddress, mint, limit, feeBuffer);
        }

        public static CoinSelection UseRandomImprove(this TransactionBodyBuilder tbb, List<Utxo> utxos, string changeAddress, ITokenBundleBuilder mint = null, int limit = 20, ulong feeBuffer = 0)
        {
            var cs = new CoinSelectionService(new RandomImproveStrategy(), new BasicChangeSelectionStrategy());
            var tb = tbb.Build();
            return cs.GetCoinSelection(tb.TransactionOutputs.ToList(), utxos, changeAddress, mint, limit, feeBuffer);
        }
    }
}
