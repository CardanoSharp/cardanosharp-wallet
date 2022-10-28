using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;

namespace CardanoSharp.Wallet.CIPs.CIP2.Extensions
{
    public static partial class TransactionOutputExtensions
    {
        public static Balance AggregateAssets(this IEnumerable<TransactionOutput> transactionOutputs, TokenBundleBuilder mint = null)
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

            // remove / add assets from balance based on mint / burn token bundle
            if (mint is not null) {
                var mintAssets = mint.Build();
                foreach (var ma  in mintAssets) {
                    foreach (var na in ma.Value.Token) {
                        var nativeAsset = balance.Assets.FirstOrDefault(x =>
                        x.PolicyId.Equals(ma.Key.ToStringHex()) && x.Name.Equals(na.Key.ToStringHex()));
                        if (nativeAsset is not null)
                        {
                            // remove native asset value from balance, if burning tokens, this will add to the balance
                            nativeAsset.Quantity = nativeAsset.Quantity - na.Value;
                            if (nativeAsset.Quantity <= 0) {
                                balance.Assets.Remove(nativeAsset);
                            }
                        }
                    }                
                }
            }            
            
            return balance;
        }
    }
}