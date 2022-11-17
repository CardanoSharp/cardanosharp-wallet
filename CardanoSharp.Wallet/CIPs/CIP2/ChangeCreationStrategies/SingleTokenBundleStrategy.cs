using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2.Extensions;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;

namespace CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies
{
    public class SingleTokenBundleStrategy: IChangeCreationStrategy
    {
        public void CalculateChange(CoinSelection coinSelection, Balance outputBalance, string changeAddress, ulong feeBuffer = 0)
        {
            //clear our change output list
            coinSelection.ChangeOutputs.Clear();

            var inputBalance = coinSelection.SelectedUtxos.AggregateAssets();

            //calculate change for token bundle
            foreach (var asset in inputBalance.Assets)
            {
                CalculateTokenBundleUtxo(coinSelection, asset, outputBalance, changeAddress);
            }

            //determine/calculate the min lovelaces required for the token bundle
            ulong minLovelaces = 0;
            if (coinSelection.ChangeOutputs.Any())
            {
                minLovelaces = coinSelection.ChangeOutputs.First().CalculateMinUtxoLovelace();
                coinSelection.ChangeOutputs.First().Value.Coin = minLovelaces;
            }

            //calculate ada utxo accounting for selected, requested, and token bundle min
            CalculateAdaUtxo(coinSelection, inputBalance.Lovelaces, minLovelaces, outputBalance, changeAddress, feeBuffer);
        }

        public void CalculateTokenBundleUtxo(CoinSelection coinSelection, Asset asset, Balance outputBalance, string changeAddress)
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

            var outputQuantity = outputBalance.Assets
                .Where(x => x.PolicyId.SequenceEqual(asset.PolicyId)
                            && x.Name.Equals(asset.Name))
                .Select(x => x.Quantity)
                .Sum();

            // determine change value for current asset based on requested and how much is selected
            var changeValue = currentQuantity - outputQuantity;
            if (changeValue <= 0)
                return;

            //since this is our token bundle change utxo, it could already exist from previous assets
            var changeUtxo = coinSelection.ChangeOutputs.FirstOrDefault(x => x.Value.MultiAsset is not null);

            if (changeUtxo is null)
            {
                //add if doesnt exist
                changeUtxo = new TransactionOutput()
                {
                    Address = new Address(changeAddress).GetBytes(),
                    Value = new TransactionOutputValue()
                    {
                        MultiAsset = new Dictionary<byte[], NativeAsset>()
                    },
                    OutputPurpose = OutputPurpose.Change
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

        public void CalculateAdaUtxo(CoinSelection coinSelection, ulong ada, ulong tokenBundleMin, Balance outputBalance, string address, ulong feeBuffer = 0)
        {
            // determine change value for current asset based on requested and how much is selected
            var changeValue = Math.Abs((long)(ada - tokenBundleMin - outputBalance.Lovelaces)) + (long)feeBuffer;
            if (changeValue <= 0)
                return;

            //this is for lovelaces
            coinSelection.ChangeOutputs.Add(new TransactionOutput()
            {
                Address = new Address(address).GetBytes(),
                Value = new TransactionOutputValue()
                {
                    Coin = (ulong)changeValue,
                    MultiAsset = null,
                },
                OutputPurpose = OutputPurpose.Change
            });
        }
    }
}
