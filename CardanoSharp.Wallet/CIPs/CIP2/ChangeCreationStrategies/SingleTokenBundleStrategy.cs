using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;

namespace CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies
{
    public class SingleTokenBundleStrategy: IChangeCreationStrategy
    {
        public void CalculateChange(CoinSelection coinSelection, Balance balance, ITokenBundleBuilder mint = null)
        {
            //clear our change output list
            coinSelection.ChangeOutputs.Clear();
            
            //calculate change for token bundle
            foreach (var asset in balance.Assets)
            {
                CalculateTokenBundleUtxo(coinSelection, asset, mint);
            }

            //determine/calculate the min lovelaces required for the token bundle
            ulong minLovelaces = 0;
            if (coinSelection.ChangeOutputs.Any())
            {
                minLovelaces = coinSelection.ChangeOutputs.First().CalculateMinUtxoLovelace();
                coinSelection.ChangeOutputs.First().Value.Coin = minLovelaces;
            }

            //calculate ada utxo accounting for selected, requested, and token bundle min 
            CalculateAdaUtxo(coinSelection, balance.Lovelaces, minLovelaces);
        }

        public void CalculateTokenBundleUtxo(CoinSelection coinSelection, Asset asset, ITokenBundleBuilder mint = null)
        {
            // get quantity of UTxO for current asset
            long currentQuantity = coinSelection.SelectedUtxos
                .Where(x => x.Balance.Assets is not null)
                .SelectMany(x => x.Balance.Assets
                    .Where(al =>
                        al.PolicyId.SequenceEqual(asset.PolicyId) 
                        && al.Name.Equals(asset.Name))
                    .Select(x => (long) x.Quantity))
                .Sum();
            
            // remove / add from currentQuantity based on mint / burn token bundle
            if (mint is not null) {
                var mintAssets = mint.Build();
                var nativeAsset = mintAssets.Where(ma => ma.Key.ToStringHex().SequenceEqual(asset.PolicyId)).FirstOrDefault().Value; 
                long mintQuantity = nativeAsset.Token.Where(na => na.Key.ToStringHex().SequenceEqual(asset.Name)).FirstOrDefault().Value;
                currentQuantity += mintQuantity;
            }            

            // determine change value for current asset based on requested and how much is selected
            var changeValue = currentQuantity - (long)asset.Quantity;
            if (changeValue <= 0) return;
            
            //since this is our token bundle change utxo, it could already exist from previous assets
            var changeUtxo = coinSelection.ChangeOutputs.FirstOrDefault(x => x.Value.MultiAsset is not null);

            if (changeUtxo is null)
            {
                //add if doesnt exist
                changeUtxo = new TransactionOutput()
                {
                    Value = new TransactionOutputValue()
                    {
                        MultiAsset = new Dictionary<byte[], NativeAsset>()
                    }
                };
                coinSelection.ChangeOutputs.Add(changeUtxo);
            }

            //determine if we already have an asset added with the same policy id
            var multiAsset = changeUtxo.Value.MultiAsset.Where(x => x.Key.SequenceEqual(asset.PolicyId.HexToByteArray()));
            if (!multiAsset.Any())
            {
                //add policy and asset to token bundle
                changeUtxo.Value.MultiAsset.Add(asset.PolicyId.HexToByteArray(), new NativeAsset()
                {
                    Token = new Dictionary<byte[], long>()
                    {
                        {asset.Name.HexToByteArray(), changeValue}
                    }
                });
            }
            else
            {
                //policy already exists in token bundle, just add the asset
                var policyAsset = multiAsset.FirstOrDefault();
                policyAsset.Value.Token.Add(asset.Name.HexToByteArray(), changeValue);
            }
        }

        public void CalculateAdaUtxo(CoinSelection coinSelection, ulong ada, ulong tokenBundleMin)
        {
            // get quantity of UTxO for current asset
            ulong currentQuantity = (ulong)coinSelection.SelectedUtxos
                    .Select(x => (long)x.Balance.Lovelaces)
                    .Sum();

            // determine change value for current asset based on requested and how much is selected
            var changeValue = Math.Abs((long)(currentQuantity - tokenBundleMin - ada));

            //this is for lovelaces
            coinSelection.ChangeOutputs.Add(new TransactionOutput()
            {
                Value = new TransactionOutputValue()
                {
                    Coin = (ulong)changeValue,
                    MultiAsset = null
                }
            });
        }
    }
}