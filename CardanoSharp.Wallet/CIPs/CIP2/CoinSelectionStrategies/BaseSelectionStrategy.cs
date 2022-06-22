using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public abstract class BaseSelectionStrategy
    {
        protected ulong GetCurrentBalance(CoinSelection coinSelection, Asset asset = null)
        {
            if (asset is null)
            {
                ulong minLovelaces = 0;
                if (coinSelection.ChangeOutputs.Any())
                {
                    minLovelaces = coinSelection.ChangeOutputs.First().CalculateMinUtxoLovelace();
                    coinSelection.ChangeOutputs.First().Value.Coin = minLovelaces;
                }
                return (ulong)coinSelection.SelectedUtxos.Sum(x => (long)x.Value) - minLovelaces;
            }
            else
            {
                return (ulong)coinSelection.SelectedUtxos.Sum(x => (long)(x.AssetList
                    .FirstOrDefault(ma =>
                        ma.PolicyId.SequenceEqual(asset.PolicyId)
                        && ma.Name.Equals(asset.Name))?.Quantity ?? 0));
            }
        }
        
        protected List<Utxo> OrderUTxOsByDescending(List<Utxo> utxos, Asset asset = null)
        {
            var orderedUtxos = new List<Utxo>();
            if (asset is null)
                orderedUtxos = utxos.OrderByDescending(x => x.Value).ToList();
            else
            {
                orderedUtxos = utxos.OrderByDescending(x => x.AssetList
                    .First(ma =>
                        ma.PolicyId.SequenceEqual(asset.PolicyId)
                        && ma.Name.Equals(asset.Name))
                    .Quantity).ToList();
            }

            return orderedUtxos;
        }
        
        protected List<Utxo> OrderUTxOsByAscending (List<Utxo> utxos, Asset asset = null)
        {
            var orderedUtxos = new List<Utxo>();
            if (asset is null)
                orderedUtxos = utxos.OrderBy(x => x.Value).ToList();
            else
            {
                orderedUtxos = utxos.OrderBy(x => x.AssetList
                    .First(ma =>
                        ma.PolicyId.SequenceEqual(asset.PolicyId)
                        && ma.Name.Equals(asset.Name))
                    .Quantity).ToList();
            }

            return orderedUtxos;
        }
    }
}