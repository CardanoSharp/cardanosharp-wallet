using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface ILargestFirstStrategy: ICoinSelectionStrategy
    {
        
    }

    public class LargestFirstStrategy: BaseSelectionStrategy, ILargestFirstStrategy
    {
        public List<TransactionUnspentOutput> SelectInputs(List<TransactionUnspentOutput> utxos, ulong amount, Asset asset = null) =>
            DetermineSelectedUtxos(amount, asset, OrderUTxOs(utxos, asset));
        

        private List<TransactionUnspentOutput> DetermineSelectedUtxos(ulong amount, Asset asset, List<TransactionUnspentOutput> orderedUtxos)
        {
            var selectedUtxos = new List<TransactionUnspentOutput>();
            ulong currentAmount = 0;
            foreach (var ou in orderedUtxos)
            {
                // if we already have enough utxos to cover requested amount, break out
                if (currentAmount >= amount) break;

                // add current item to selected UTxOs
                selectedUtxos.Add(ou);

                // get quantity of UTxO
                var quantity = (asset is null)
                    ? ou.Output.Value.Coin
                    : ou.Output.Value.MultiAsset[asset.PolicyId].Token[asset.Name];

                // increment current amount by the UTxO quantity
                currentAmount = currentAmount + quantity;
            }

            return selectedUtxos;
        }
    }
}