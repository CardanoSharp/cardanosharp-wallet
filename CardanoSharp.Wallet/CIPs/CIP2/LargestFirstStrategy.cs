using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface ILargestFirstStrategy: ICoinSelectionStrategy
    {
        
    }

    public class LargestFirstStrategy: ILargestFirstStrategy
    {
        public List<TransactionUnspentOutput> SelectInputs(List<TransactionUnspentOutput> utxos, ulong amount, Asset asset = null)
        {
            if(asset is null)
                return utxos.OrderByDescending(x => x.Output.Value.Coin).ToList();
            else
            {
                return utxos.OrderByDescending(x => x.Output.Value.MultiAsset
                    .First(ma =>
                        ma.Key.SequenceEqual(asset.PolicyId)
                        && ma.Value.Token.ContainsKey(asset.Name))
                    .Value.Token[asset.Name]).ToList();
            }
        }
    }
}