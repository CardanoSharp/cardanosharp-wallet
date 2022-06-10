using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface IRandomImproveStrategy: ICoinSelectionStrategy
    {
        
    }
    
    public class RandomImproveStrategy: BaseSelectionStrategy, IRandomImproveStrategy
    {
        public (List<Utxo> inputs, List<TransactionOutput> changes) SelectInputs(List<TransactionOutput> outputs, List<Utxo> availableUtxos)
        {
            var selectedUTxOs = new List<Utxo>();
            //1. Randomly select UTxOs
            var rand = new Random();
            ulong currentAmount = 0;
            var availableUTxOs = OrderUTxOsByDescending(new List<Utxo>(utxos), asset);
            while (currentAmount < amount && availableUTxOs.Any())
            {
                var availableLength = availableUTxOs.Count();
                var randomIndex = rand.Next(availableLength - 1);
                var randomUTxO = availableUTxOs[randomIndex];

                //if the random utxo does not have the asset we are trying to select
                if (asset is not null 
                    && randomUTxO.AssetList.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)) is null)
                {
                    availableUTxOs.RemoveAt(randomIndex);
                    continue;
                }
                
                selectedUTxOs.Add(randomUTxO);
                availableUTxOs.RemoveAt(randomIndex);
                
                // get quantity of UTxO
                var quantity = (asset is null)
                    ? randomUTxO.Value
                    : randomUTxO.AssetList.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity;

                // increment current amount by the UTxO quantity
                currentAmount = currentAmount + quantity;
            }
            
            //2. Improve by expanding selection
            // https://cips.cardano.org/cips/cip2/#random-improve
            var min = amount;
            var ideal = min * 2;
            var max = min * 3;
            ulong idealAmountTOCalculate = 0;
            ulong accumulatedCoinSelection = 0;

            var previousOutput = new Utxo();
            var idealUtxoSet = new List<Utxo>();


            foreach (var utxoToEvaluate in OrderUTxOsByAscending(availableUTxOs, asset))
            {
                //The next 3 conditions establish whether the utxo is an "improvement" and should be added to the change
                
                if(!CalculateCondition(utxoToEvaluate, ideal, max, previousOutput, idealUtxoSet, utxos, asset)) continue;

                var changeValue = asset is null ? utxoToEvaluate.Value - min :  
                    utxoToEvaluate.AssetList.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity - min;

                //TODO need to return the accumulated CoinSelection as an output to the outputs field
                accumulatedCoinSelection += changeValue;

                selectedUTxOs.Add(utxoToEvaluate);
            }

            return (selectedUTxOs, new List<TransactionOutput>());
        }

        private static bool CalculateCondition(Utxo utxoToEvaluate, ulong ideal, ulong max, Utxo previousOutput,  ICollection idealUtxoSet, ICollection utxos, Asset asset)
        {
            bool[] arrayToMatchConditions = { false, false, false,};
            arrayToMatchConditions[0] =  asset is null ? Math.Abs((long) (ideal - utxoToEvaluate.Value)) < Math.Abs((long) (ideal -  previousOutput.Value)) 
                : Math.Abs((long) (ideal - utxoToEvaluate.AssetList.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity)) < 
                  Math.Abs((long) (ideal - previousOutput.AssetList.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity)) ;

            arrayToMatchConditions[1] = asset is null ? utxoToEvaluate.Value <= max : utxoToEvaluate.AssetList.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity <= max;

            //TODO how do we define the maximum input count as listed below
            //Condition 3: when counting cumulatively across all outputs considered so far, we have not selected more than the maximum number of UTxO entries specified by Maximum Input Count.
            arrayToMatchConditions[2] = idealUtxoSet.Count <= utxos.Count;
            
            return arrayToMatchConditions[0] || arrayToMatchConditions[1] || arrayToMatchConditions[2];
        }
        
    }
    
}