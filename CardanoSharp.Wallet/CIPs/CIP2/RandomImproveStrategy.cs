using System;
using System.Linq;
using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface IRandomImproveStrategy: ICoinSelectionStrategy
    {
        
    }
    
    public class RandomImproveStrategy: BaseSelectionStrategy, IRandomImproveStrategy
    {
        public List<TransactionUnspentOutput> SelectInputs(List<TransactionUnspentOutput> utxos, ulong amount, Asset asset = null)
        {
            var selectedUTxOs = new List<TransactionUnspentOutput>();
            //1. Randomly select UTxOs
            var rand = new Random();
            ulong currentAmount = 0;
            var availableUTxOs = OrderUTxOsByDescending(new List<TransactionUnspentOutput>(utxos), asset);
            while (currentAmount < amount && availableUTxOs.Any())
            {
                var availableLength = availableUTxOs.Count();
                var randomIndex = rand.Next(availableLength - 1);
                var randomUTxO = availableUTxOs[randomIndex];

                //if the random utxo does not have the asset we are trying to select
                if (asset is not null 
                    && randomUTxO.Output.Value.MultiAsset[asset.PolicyId]?.Token[asset.Name] is null)
                {
                    availableUTxOs.RemoveAt(randomIndex);
                    continue;
                }
                
                selectedUTxOs.Add(randomUTxO);
                availableUTxOs.RemoveAt(randomIndex);
                
                // get quantity of UTxO
                var quantity = (asset is null)
                    ? randomUTxO.Output.Value.Coin
                    : randomUTxO.Output.Value.MultiAsset[asset.PolicyId].Token[asset.Name];

                // increment current amount by the UTxO quantity
                currentAmount = currentAmount + quantity;
            }

            //2. Improve by expanding selection
            // https://cips.cardano.org/cips/cip2/#random-improve
            var min = amount;
            var ideal = min * 2;
            var max = min * 3;
            ulong idealAmountTOCalculate = 0;
            bool[] arrayToMatchConditions = { false, false, false,};
            var previousOutput = new TransactionUnspentOutput(); 
            selectedUTxOs = OrderUTxOsByAscending(selectedUTxOs, asset);
            var idealUtxoSet = new List<TransactionUnspentOutput>();
            while (selectedUTxOs.Count > 0)
            {
                var utxoToEvaluate = selectedUTxOs[rand.Next(selectedUTxOs.Count)];

                arrayToMatchConditions[0] = Math.Abs((long) (ideal - utxoToEvaluate.Output.Value.Coin)) < Math.Abs((long) (ideal -  previousOutput.Output.Value.Coin));

                arrayToMatchConditions[1] = utxoToEvaluate.Output.Value.Coin <= max;

                arrayToMatchConditions[2] = idealUtxoSet.Count <= utxos.Count;
                
                if(!arrayToMatchConditions[0] && !arrayToMatchConditions[1] && !arrayToMatchConditions[2]) continue;

                var changeValue = (idealUtxoSet.Sum(x => (long)x.Output.Value.Coin)) - (long)utxoToEvaluate.Output.Value.Coin;
                
                idealUtxoSet.Add(utxoToEvaluate);

                previousOutput = utxoToEvaluate;
            }
            

            return selectedUTxOs;
        }
    }
}