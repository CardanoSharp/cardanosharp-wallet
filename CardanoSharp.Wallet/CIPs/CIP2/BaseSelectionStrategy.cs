using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public abstract class BaseSelectionStrategy
    {
        protected List<TransactionUnspentOutput> OrderUTxOsByDescending(List<TransactionUnspentOutput> utxos, Asset asset = null)
        {
            var orderedUtxos = new List<TransactionUnspentOutput>();
            if (asset is null)
                orderedUtxos = utxos.OrderByDescending(x => x.Output.Value.Coin).ToList();
            else
            {
                orderedUtxos = utxos.OrderByDescending(x => x.Output.Value.MultiAsset
                    .First(ma =>
                        ma.Key.SequenceEqual(asset.PolicyId)
                        && ma.Value.Token.ContainsKey(asset.Name))
                    .Value.Token[asset.Name]).ToList();
            }

            return orderedUtxos;
        }
        
        protected List<TransactionUnspentOutput> OrderUTxOsByAscending (List<TransactionUnspentOutput> utxos, Asset asset = null)
        {
            var orderedUtxos = new List<TransactionUnspentOutput>();
            if (asset is null)
                orderedUtxos = utxos.OrderBy(x => x.Output.Value.Coin).ToList();
            else
            {
                orderedUtxos = utxos.OrderBy(x => x.Output.Value.MultiAsset
                    .First(ma =>
                        ma.Key.SequenceEqual(asset.PolicyId)
                        && ma.Value.Token.ContainsKey(asset.Name))
                    .Value.Token[asset.Name]).ToList();
            }

            return orderedUtxos;
        }
    }
}