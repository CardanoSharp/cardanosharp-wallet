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

        public CoinSelectionResponse GetInputs(List<TransactionOutput> outputs, List<Utxo> utxos)
        {
            //this seems like an odd exception the more i see it.
            // if (!HasRequiredAsset(utxos, asset))
            //     throw new Exception("UTxOs do not contain a required asset");

            (var selectedInputs, var changeOutputs) =
                _coinSelection.SelectInputs(outputs, utxos);

            //good but needs to move to the strategies
            // if (!HasSufficientBalance(selectedInputs, asset.Quantity, asset.PolicyId is null ? null : asset))
            //     throw new Exception("UTxOs have insufficient balance");
            
            return new CoinSelectionResponse()
            {
                Inputs = selectedInputs.GetTransactionInputs(),
                ChangeOutputs = changeOutputs
            };
        }

        private bool HasRequiredAsset(IEnumerable<Utxo> utxos, Asset asset = null)
        {
            if (asset is not null)
                return utxos.Any(x =>
                    x.AssetList.Any(ma =>
                        ma.PolicyId.SequenceEqual(asset.PolicyId)
                        && ma.Name.Equals(asset.Name)));

            return true;
        }

        private bool HasSufficientBalance(IEnumerable<Utxo> selectedUtxos, ulong amount, Asset asset = null)
        {
            ulong totalInput = 0;
            foreach (var su in selectedUtxos)
            {
                ulong quantity = 0;
                if (asset is null)
                {
                    quantity = su.Value;
                }
                else
                {
                    quantity = su.AssetList
                        .First(ma =>
                            ma.PolicyId.SequenceEqual(asset.PolicyId)
                            && ma.Name.Equals(asset.Name))
                        .Quantity;
                }

                totalInput = totalInput + quantity;
            }

            return totalInput >= amount;
        }
    }
}