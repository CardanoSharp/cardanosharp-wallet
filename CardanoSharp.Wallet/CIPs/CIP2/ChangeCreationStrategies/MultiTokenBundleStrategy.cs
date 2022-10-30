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

namespace CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies
{
    public class MultiTokenBundleStrategy: IChangeCreationStrategy
    {
        public void CalculateChange(CoinSelection coinSelection, Balance outputBalance)
        {
            //clear our change output list
            coinSelection.ChangeOutputs.Clear();

            var inputBalance = coinSelection.SelectedUtxos.AggregateAssets();
            
            // ideal change outputs is 4 because the max output size is 5kb
            // if the max tx size is 16kb, 4 token bundle outputs prevents
            // all output size errors
            int idealTokenBundleChangeOutputs = 4;
            var tokenBundleChangeOutputs = new List<TransactionOutput>();

            //calculate change for token bundle
            int changeOutputIndex = 0;
            foreach (var asset in inputBalance.Assets)
            {
                TransactionOutput changeOutput = null;
                if (changeOutputIndex < tokenBundleChangeOutputs.Count)
                    changeOutput = tokenBundleChangeOutputs[changeOutputIndex];

                TransactionOutput calculatedOutput = CalculateTokenBundleUtxo(coinSelection, asset, outputBalance, changeOutput);
                if (tokenBundleChangeOutputs.Count < idealTokenBundleChangeOutputs)
                    tokenBundleChangeOutputs.Add(calculatedOutput);
                else
                    tokenBundleChangeOutputs[changeOutputIndex] = calculatedOutput;

                changeOutputIndex += 1;
                if (changeOutputIndex >= idealTokenBundleChangeOutputs)
                    changeOutputIndex = 0;
            }

            //determine/calculate the min lovelaces required for the token bundle
            ulong minLovelaces = 0;
            foreach (var changeOutput in tokenBundleChangeOutputs) {
                ulong changeLovelaces = changeOutput.CalculateMinUtxoLovelace();
                minLovelaces += changeLovelaces;
                changeOutput.Value.Coin = changeLovelaces;
            }
            coinSelection.ChangeOutputs = tokenBundleChangeOutputs;            

            //calculate ada utxo accounting for selected, requested, and token bundle min 
            CalculateAdaUtxo(coinSelection, inputBalance.Lovelaces, minLovelaces, outputBalance);
        }

        public TransactionOutput CalculateTokenBundleUtxo(CoinSelection coinSelection, Asset asset, Balance outputBalance, TransactionOutput changeUtxo)
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

            if (changeUtxo is null)
            {
                //add if doesnt exist
                changeUtxo = new TransactionOutput()
                {
                    Value = new TransactionOutputValue()
                    {
                        MultiAsset = new Dictionary<byte[], NativeAsset>()
                    },
                    OutputPurpose = OutputPurpose.Change
                };
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

            return changeUtxo;
        }

        public void CalculateAdaUtxo(CoinSelection coinSelection, ulong ada, ulong tokenBundleMin, Balance outputBalance)
        {
            // determine change value for current asset based on requested and how much is selected
            var changeValue = Math.Abs((long)(ada - tokenBundleMin - outputBalance.Lovelaces));

            //this is for lovelaces
            coinSelection.ChangeOutputs.Add(new TransactionOutput()
            {
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