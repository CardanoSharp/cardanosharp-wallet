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
            var totalSelectedUtxos = new CoinSelectionResponse()
            {
                Inputs = new List<TransactionInput>(),
                ChangeOutputs = new List<TransactionOutput>()
            };

            foreach (var asset in outputs.AggregateAssets())
            {
                if (!HasRequiredAsset(utxos, asset))
                    throw new Exception("UTxOs do not contain a required asset");

                var selectedInputs =
                    _coinSelection.SelectInputs(utxos, asset.Quantity, asset.PolicyId is null ? null : asset);

                if (!HasSufficientBalance(selectedInputs, asset.Quantity, asset.PolicyId is null ? null : asset))
                    throw new Exception("UTxOs have insufficient balance");

                var changeOutputs =
                    _coinSelection.CreateChange(utxos, asset.Quantity, asset.PolicyId is null ? null : asset);
                totalSelectedUtxos.Inputs.AddRange(selectedInputs.GetTransactionInputs());
                totalSelectedUtxos.ChangeOutputs.AddRange(changeOutputs);
            }

            return totalSelectedUtxos;
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