using System;
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
        public List<Utxo> SelectInputs(List<Utxo> utxos, ulong amount, Asset asset = null)
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
            bool[] arrayToMatchConditions = { false, false, false,};
            var previousOutput = new Utxo();
            var idealUtxoSet = new List<Utxo>();


            foreach (var utxoToEvaluate in OrderUTxOsByAscending(utxos, asset))
            {
                //The next 3 conditions establish whether the utxo is an "improvement" and should be added to the change
                arrayToMatchConditions[0] = Math.Abs((long) (ideal - utxoToEvaluate.Value)) < Math.Abs((long) (ideal -  previousOutput.Value));

                arrayToMatchConditions[1] = utxoToEvaluate.Value <= max;

                arrayToMatchConditions[2] = idealUtxoSet.Count <= utxos.Count;
                
                if(!arrayToMatchConditions[0] && !arrayToMatchConditions[1] && !arrayToMatchConditions[2]) continue;

                var changeValue = (idealUtxoSet.Sum(x => (long)x.Value)) - (long)utxoToEvaluate.Value;
                
                idealUtxoSet.Add(utxoToEvaluate);

                previousOutput = utxoToEvaluate;
            }

            return selectedUTxOs;
        }

        public List<TransactionOutput> CreateChange(List<Utxo> utxos, ulong amount, Asset asset = null)
        {

            return new List<TransactionOutput>();
        }
        
    }
}