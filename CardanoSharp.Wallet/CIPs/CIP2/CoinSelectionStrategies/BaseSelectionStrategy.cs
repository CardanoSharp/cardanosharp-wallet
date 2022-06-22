using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public abstract class BaseSelectionStrategy
    {
        protected ulong GetCurrentBalance(List<Utxo> selectedUtxos, Asset asset = null)
        {
            if (asset is null)
            {
                return (ulong)selectedUtxos.Sum(x => (long)x.Value);
            }
            else
            {
                return (ulong)selectedUtxos.Sum(x => (long)(x.AssetList
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