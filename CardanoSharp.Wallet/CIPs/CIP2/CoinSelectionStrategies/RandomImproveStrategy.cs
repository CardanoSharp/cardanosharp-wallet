using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.TransactionBuilding;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface IRandomImproveStrategy: ICoinSelectionStrategy
    {
        
    }
    
    public class RandomImproveStrategy: BaseSelectionStrategy, IRandomImproveStrategy
    {
        public void SelectInputs(CoinSelection coinSelection, List<Utxo> availableUtxos, long amount, Asset asset = null, int limit = 20)
        {
            //1. Randomly select UTxOs
            var rand = new Random();
            
            //determine
            long currentAmount = GetCurrentBalance(coinSelection, asset);
            
            //reorder list
            List<Utxo> descendingAvailableUtxos = OrderUTxOsByDescending(availableUtxos, asset);
            
            //create a temporary selected utxo list
            var currentSelectedUtxo = new List<Utxo>();
            
            while (currentAmount < amount && descendingAvailableUtxos.Any())
            {
                // make sure we havent added too many utxos
                // we minus 1 here because we will improve which will add an additional utxo
                if (currentSelectedUtxo.Count() + coinSelection.SelectedUtxos.Count() >= limit - 1)
                {
                    // If it is not enough amount, clear the current and existing selections
                    if (currentAmount < amount)
                    {
                        currentSelectedUtxo.Clear();
                        coinSelection.Clear();
                        currentAmount = 0;
                    }
                    else
                        break;
                }
            
                var availableLength = descendingAvailableUtxos.Count();
                var randomIndex = rand.Next(availableLength - 1);
                var randomUTxO = descendingAvailableUtxos[randomIndex];

                //if the random utxo does not have the asset we are trying to select
                if (asset is not null 
                    && randomUTxO.Balance.Assets.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)) is null)
                {
                    descendingAvailableUtxos.RemoveAt(randomIndex);
                    continue;
                }
                
                currentSelectedUtxo.Add(randomUTxO);
                descendingAvailableUtxos.RemoveAt(randomIndex);
                
                // get quantity of UTxO
                var quantity = (asset is null)
                    ? (long)randomUTxO.Balance.Lovelaces
                    : randomUTxO.Balance.Assets.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity;

                // increment current amount by the UTxO quantity
                currentAmount = currentAmount + quantity;
            }
            
            //2. Improve by expanding selection
            // https://cips.cardano.org/cips/cip2/#random-improve
            var min = amount;
            var ideal = min * 2;
            var max = min * 3;

            var v0 = new Utxo();
            
            foreach (var v1 in OrderUTxOsByAscending(descendingAvailableUtxos, asset))
            {
                //The next 3 conditions establish whether the utxo is an "improvement" and should be added to the change
                if(!CalculateCondition(v1, ideal, max, v0, limit, currentSelectedUtxo, asset)) continue;

                currentSelectedUtxo.Add(v1);
                if(v0 is not null) currentSelectedUtxo.Remove(v0);
                
                v0 = v1;
            }

            coinSelection.SelectedUtxos.AddRange(currentSelectedUtxo);

            //remove the utxos we used
            currentSelectedUtxo.ForEach(x => availableUtxos.Remove(x));
        }

        private bool CalculateCondition(Utxo v1, long ideal, long max, Utxo v0, int limit, ICollection utxos, Asset asset)
        {
            // Condition 1: we have moved closer to the ideal value
            bool[] arrayToMatchConditions = { false, false, false,};
            arrayToMatchConditions[0] =  asset is null ? Math.Abs((long) (ideal - (long)v1.Balance.Lovelaces)) < Math.Abs((long) (ideal -  (long)(v0.Balance?.Lovelaces ?? 0))) 
                : Math.Abs((long) (ideal - v1.Balance.Assets.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity)) < 
                  Math.Abs((long) (ideal - (v0.Balance?.Assets?.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name))?.Quantity ?? 0))) ;

            // Condition 2: we have not exceeded the maximum value
            arrayToMatchConditions[1] = asset is null ? (long)v1.Balance.Lovelaces <= max : v1.Balance.Assets.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity <= max;

            //Condition 3: when counting cumulatively across all outputs considered so far, we have not selected more than the maximum number of UTxO entries specified by Maximum Input Count.
            arrayToMatchConditions[2] = limit > utxos.Count;
            
            return arrayToMatchConditions[0] && arrayToMatchConditions[1] && arrayToMatchConditions[2];
        }
    }
    
}