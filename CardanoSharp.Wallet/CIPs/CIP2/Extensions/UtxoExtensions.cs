using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Models;

namespace CardanoSharp.Wallet.CIPs.CIP2.Extensions
{
    public static partial class UtxoExtensions
    {
        public static Balance AggregateAssets(this IEnumerable<Utxo> utxos)
        {
            Balance balance = new Balance()
            {
                Assets = new List<Asset>()
            };

            foreach (var o in utxos)
            {
                //aggregate lovelaces
                balance.Lovelaces = balance.Lovelaces + o.Balance.Lovelaces;

                //aggregate native assets
                if(o.Balance.Assets is null) continue;
                
                foreach (var ma in o.Balance.Assets)
                {
                    var nativeAsset = balance.Assets.FirstOrDefault(x =>
                        x.PolicyId.Equals(ma.PolicyId) && x.Name.Equals(ma.Name));
                    if (nativeAsset is null)
                    {
                        nativeAsset = new Asset()
                        {
                            PolicyId = ma.PolicyId,
                            Name = ma.Name,
                            Quantity = 0
                        };
                        balance.Assets.Add(nativeAsset);
                    }

                    nativeAsset.Quantity = nativeAsset.Quantity + ma.Quantity;
                    
                }
            }
            
            return balance;
        }
    }
}