using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface ILargestFirstStrategy: ICoinSelectionStrategy
    {
        
    }

    public class LargestFirstStrategy: BaseSelectionStrategy, ILargestFirstStrategy
    {
        public void SelectInputs(CoinSelection coinSelection, List<Utxo> availableUtxos, long amount, Asset asset = null, int limit = 20)
        {
            //determine
            long currentAmount = GetCurrentBalance(coinSelection, asset);
            
            //reorder the available utxos
            availableUtxos = OrderUTxOsByDescending(availableUtxos, asset);
            
            //indices to remove
            var removeIndices = new List<Utxo>();
            
            for(var x = 0; x < availableUtxos.Count(); x++)
            {
                var ou = availableUtxos[x];
                
                // if we already have enough utxos to cover requested amount, break out
                if (currentAmount > amount) break;
                
                // make sure we havent added too many utxos
                if (coinSelection.SelectedUtxos.Count() == limit) break;

                // add current item to selected UTxOs
                coinSelection.SelectedUtxos.Add(ou);
                removeIndices.Add(ou);

                // get quantity of UTxO
                var quantity = (asset is null)
                    ? (long)ou.Balance.Lovelaces
                    : ou.Balance.Assets.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity;

                // increment current amount by the UTxO quantity
                currentAmount = currentAmount + quantity;
            }
            
            //remove the utxos we used
            removeIndices.ForEach(x => availableUtxos.Remove(x));
        }
    }
}