using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface ILargestFirstStrategy: ICoinSelectionStrategy
    {
        
    }

    public class LargestFirstStrategy: BaseSelectionStrategy, ILargestFirstStrategy
    {
        public List<TransactionOutput> CreateChange(List<Utxo> utxos, ulong amount, Asset asset = null)
        {
            throw new System.NotImplementedException();
        }

        public List<Utxo> SelectInputs(List<Utxo> utxos, ulong amount, Asset asset)
        {
            var selectedUtxos = new List<Utxo>();
            ulong currentAmount = 0;
            foreach (var ou in OrderUTxOsByDescending(utxos))
            {
                // if we already have enough utxos to cover requested amount, break out
                if (currentAmount >= amount) break;

                // add current item to selected UTxOs
                selectedUtxos.Add(ou);

                // get quantity of UTxO
                var quantity = (asset is null)
                    ? ou.Value
                    : ou.AssetList.FirstOrDefault(x => x.PolicyId.SequenceEqual(asset.PolicyId) && x.Name.Equals(asset.Name)).Quantity;

                // increment current amount by the UTxO quantity
                currentAmount = currentAmount + quantity;
            }

            return selectedUtxos;
        }
    }
}