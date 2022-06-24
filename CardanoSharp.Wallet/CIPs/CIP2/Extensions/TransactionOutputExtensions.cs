using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2.Extensions
{
    public static partial class TransactionOutputExtensions
    {
        public static Balance AggregateAssets(this IEnumerable<TransactionOutput> transactionOutputs)
        {
            Balance balance = new Balance()
            {
                Assets = new List<Asset>()
            };

            foreach (var o in transactionOutputs)
            {
                //aggregate lovelaces
                balance.Lovelaces = balance.Lovelaces + o.Value.Coin;

                //aggregate native assets
                if(o.Value.MultiAsset is null) continue;
                
                foreach (var ma in o.Value.MultiAsset)
                {
                    foreach (var na in ma.Value.Token)
                    {
                        var nativeAsset = balance.Assets.FirstOrDefault(x =>
                            x.PolicyId.Equals(ma.Key.ToStringHex()) && x.Name.Equals(na.Key.ToStringHex()));
                        if (nativeAsset is null)
                        {
                            nativeAsset = new Asset()
                            {
                                PolicyId = ma.Key.ToStringHex(),
                                Name = na.Key.ToStringHex(),
                                Quantity = 0
                            };
                            balance.Assets.Add(nativeAsset);
                        }

                        nativeAsset.Quantity = nativeAsset.Quantity + na.Value;
                    }
                }
            }
            
            return balance;
        }
    }
}