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
            var availableUTxOs = OrderUTxOs(new List<TransactionUnspentOutput>(utxos), asset);
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
            var min = amount;
            var ideal = min * 2;
            var max = min * 3;
            while ()
            {
                
            }

            return selectedUTxOs;
        }
    }
}