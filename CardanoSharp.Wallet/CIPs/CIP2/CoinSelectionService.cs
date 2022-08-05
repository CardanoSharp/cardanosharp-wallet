using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies;
using CardanoSharp.Wallet.CIPs.CIP2.Extensions;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface ICoinSelectionService
    {
        CoinSelection GetCoinSelection(IEnumerable<TransactionOutput> outputs, IEnumerable<Utxo> utxos, int limit = 20);
    }
    
    public class CoinSelectionService: ICoinSelectionService
    {
        private readonly ICoinSelectionStrategy _coinSelection;
        private readonly IChangeCreationStrategy _changeCreation;

        public CoinSelectionService(ICoinSelectionStrategy coinSelection, IChangeCreationStrategy changeCreation)
        {
            _coinSelection = coinSelection;
            _changeCreation = changeCreation;
        }

        public CoinSelection GetCoinSelection(IEnumerable<TransactionOutput> outputs, IEnumerable<Utxo> utxos, int limit = 20)
        {
            var coinSelection = new CoinSelection();
            var availableUTxOs = new List<Utxo>(utxos);

            var balance = outputs.AggregateAssets();
            
            foreach (var asset in balance.Assets)
            {
                _coinSelection.SelectInputs(coinSelection, availableUTxOs, asset.Quantity, asset, limit);

                if (!HasSufficientBalance(coinSelection.SelectedUtxos, asset.Quantity, asset))
                    throw new Exception("UTxOs have insufficient balance");
            }
            
            //we need to determine if we have any change for tokens. this way we can accommodate the min lovelaces in our current value
            if(coinSelection.SelectedUtxos.Any() && _changeCreation is not null) _changeCreation.CalculateChange(coinSelection, balance);
            
            _coinSelection.SelectInputs(coinSelection, availableUTxOs, (long)balance.Lovelaces, null, limit);
            
            if (!HasSufficientBalance(coinSelection.SelectedUtxos, (long)balance.Lovelaces, null))
                throw new Exception("UTxOs have insufficient balance");
            
            if(_changeCreation is not null) _changeCreation.CalculateChange(coinSelection, balance);

            PopulateInputList(coinSelection);

            return coinSelection;
        }

        private bool HasSufficientBalance(IEnumerable<Utxo> selectedUtxos, long amount, Asset asset = null)
        {
            long totalInput = 0;
            foreach (var su in selectedUtxos)
            {
                long quantity = 0;
                if (asset is null)
                {
                    quantity = (long)su.Balance.Lovelaces;
                }
                else
                {
                    quantity = su.Balance.Assets
                        .First(ma =>
                            ma.PolicyId.SequenceEqual(asset.PolicyId)
                            && ma.Name.Equals(asset.Name))
                        .Quantity;
                }

                totalInput = totalInput + quantity;
            }

            return totalInput >= amount;
        }

        private void PopulateInputList(CoinSelection coinSelection)
        {
            foreach (var su in coinSelection.SelectedUtxos)
            {
                coinSelection.Inputs.Add(new TransactionInput()
                {
                    TransactionId = su.TxHash.HexToByteArray(),
                    TransactionIndex = su.TxIndex
                });
            }
        }
    }
}