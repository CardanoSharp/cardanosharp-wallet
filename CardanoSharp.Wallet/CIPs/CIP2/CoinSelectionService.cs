using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public class CoinSelectionService
    {
        private readonly ICoinSelectionStrategy _coinSelection;

        public CoinSelectionService(ICoinSelectionStrategy coinSelection)
        {
            _coinSelection = coinSelection;
        }

        public List<TransactionUnspentOutput> GetInputs(List<TransactionOutput> outputs, List<TransactionUnspentOutput> utxos)
        {
            List<TransactionUnspentOutput> totalSelectedUtxos = new List<TransactionUnspentOutput>();

            foreach (var asset in outputs.AggregateAssets())
            {
                if (!HasRequiredAsset(utxos, asset))
                    throw new Exception("UTxOs do not have contain a required asset");

                var selectedUtxos = _coinSelection.SelectInputs(utxos, asset.Quantity, asset.PolicyId is null ? null : asset);

                if (!HasSufficientBalance(selectedUtxos, asset.Quantity, asset.PolicyId is null ? null : asset))
                    throw new Exception("UTxOs have insufficient balance");
                
                totalSelectedUtxos.AddRange(selectedUtxos);
            }

            return totalSelectedUtxos.ToList();
        }

        private bool HasRequiredAsset(IEnumerable<TransactionUnspentOutput> utxos, Asset asset = null)
        {
            if (asset is not null)
                return utxos.Any(x =>
                    x.Output.Value.MultiAsset.Any(ma =>
                        ma.Key.SequenceEqual(asset.PolicyId)
                        && ma.Value.Token.ContainsKey(asset.Name)));
            
            return true;
        }

        private bool HasSufficientBalance(IEnumerable<TransactionUnspentOutput> selectedUtxos, ulong amount, Asset asset = null)
        {
            ulong totalInput = 0;
            foreach (var su in selectedUtxos)
            {
                ulong quantity = 0;
                if (asset is null)
                {
                    quantity = su.Output.Value.Coin;
                }else
                {
                    quantity = su.Output.Value.MultiAsset
                        .First(ma =>
                            ma.Key.SequenceEqual(asset.PolicyId)
                            && ma.Value.Token.ContainsKey(asset.Name))
                        .Value.Token[asset.Name];
                }
                
                totalInput = totalInput + quantity;
            }

            return totalInput >= amount;
        }
    }
}