using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public class CoinSelectionService
    {
        private readonly ICoinSelectionStrategy _coinSelection;
        private readonly IChangeCreationStrategy _changeCreation;

        public CoinSelectionService(ICoinSelectionStrategy coinSelection, IChangeCreationStrategy changeCreation)
        {
            _coinSelection = coinSelection;
            _changeCreation = changeCreation;
        }

        public CoinSelection GetCoinSelection(List<TransactionOutput> outputs, List<Utxo> utxos, int limit = 20)
        {
            var coinSelection = new CoinSelection();
            var availableUTxOs = new List<Utxo>(utxos);
            
            foreach (var asset in outputs.AggregateAssets())
            {
                _coinSelection.SelectInputs(coinSelection, availableUTxOs, asset.Quantity, asset.PolicyId is null ? null : asset, limit);

                //good but needs to move to the strategies
                if (!HasSufficientBalance(coinSelection.SelectedUtxos, asset.Quantity, asset.PolicyId is null ? null : asset))
                    throw new Exception("UTxOs have insufficient balance");
            }
            
            if(_changeCreation is not null) _changeCreation.CalculateChange(coinSelection, outputs.AggregateAssets());

            return coinSelection;
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