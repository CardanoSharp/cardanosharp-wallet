using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2;
using CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;
using Xunit;

namespace CardanoSharp.Wallet.Test.CIPs;

public partial class CIP2Tests
{
     [Fact]
    public void LargestFirst_SingleOutput_Burn_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_1_burned_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_1_owned_mint_asset,
            utxo_40_ada_no_assets
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_40_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count(), 2);
        Assert.Equal(response.ChangeOutputs.Count(), 2);

        var selectedUTXOsSum = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
                x.Balance.Assets.Where(y => 
                    y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId))
                        ?.Sum(z => (long)z.Quantity) ?? 0);

        var changeOutputSum = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        
        var outputsSum = outputs.Sum(x =>
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        Assert.Equal(selectedUTXOsSum + outputsSum + changeOutputSum, 0);
    }

    [Fact]
    public void LargestFirst_MultiOutput_Burn_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_1_ada_no_assets, output_10_ada_1_burned_assets, output_1_ada_no_assets, output_1_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_30_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset,
            utxo_40_ada_no_assets
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_40_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count(), 2);
        Assert.Equal(response.ChangeOutputs.Count(), 2);

        var selectedUTXOsSum = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
                x.Balance.Assets.Where(y => 
                    y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId))
                        ?.Sum(z => (long)z.Quantity) ?? 0);

        var changeOutputSum = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        
        var outputsSum = outputs.Sum(x =>
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        Assert.Equal(selectedUTXOsSum + outputsSum + changeOutputSum, 0);
    }

    [Fact]
    public void LargestFirst_SingleOutput_MultiBurn_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_burned_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_1_owned_mint_asset_two
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count(), 2);
        Assert.Equal(response.ChangeOutputs.Count(), 2);

        var selectedUTXOsSumAsset1 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);
        var outputsSumAsset1 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        var selectedUTXOsSumAsset2 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);
        var outputsSumAsset2 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        var changeOutputSumAsset1 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        var changeOutputSumAsset2 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        
        Assert.Equal(selectedUTXOsSumAsset1 + outputsSumAsset1 + changeOutputSumAsset1, 0);
        Assert.Equal(selectedUTXOsSumAsset2 + outputsSumAsset2 + changeOutputSumAsset2, 0);
    }

    [Fact]
    public void LargestFirst_MultiOutput_MultiBurn_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_burned_assets, output_10_ada_2_burned_assets, output_10_ada_1_burned_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_1_owned_mint_asset_two,
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_1_owned_mint_asset_two,
            utxo_10_ada_1_owned_mint_asset,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[2].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[3].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[4].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[4].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count(), 5);
        Assert.Equal(response.ChangeOutputs.Count(), 2);

        var selectedUTXOsSumAsset1 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);
        var outputsSumAsset1 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        var selectedUTXOsSumAsset2 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);
        var outputsSumAsset2 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        var changeOutputSumAsset1 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        var changeOutputSumAsset2 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        
        Assert.Equal(selectedUTXOsSumAsset1 + outputsSumAsset1 + changeOutputSumAsset1, 0);
        Assert.Equal(selectedUTXOsSumAsset2 + outputsSumAsset2 + changeOutputSumAsset2, 0);
    }

    [Fact]
    public void LargestFirst_MultiUtxo_MultiOutput_MultiBurn_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_burned_assets, output_10_ada_50_tokens, output_100_ada_no_assets, output_10_ada_1_already_minted_assets, output_10_ada_2_burned_assets, output_10_ada_1_burned_assets };
        var utxos = new List<Utxo>()
        {
            utxo_50_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset_two,
            utxo_10_ada_1_owned_mint_asset,
            utxo_70_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset_two,
            utxo_10_ada_40_tokens,
            utxo_10_ada_20_tokens,
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_1_owned_mint_asset,
            utxo_60_ada_no_assets,
            utxo_10_ada_10_tokens,
            utxo_10_ada_1_owned_mint_asset,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[2].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[3].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[4].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[4].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[5].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[5].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[6].TxHash, utxo_10_ada_40_tokens.TxHash);
        Assert.Equal(response.Inputs[6].TransactionId, utxo_10_ada_40_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[7].TxHash, utxo_10_ada_20_tokens.TxHash);
        Assert.Equal(response.Inputs[7].TransactionId, utxo_10_ada_20_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[8].TxHash, utxo_70_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[8].TransactionId, utxo_70_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[9].TxHash, utxo_60_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[9].TransactionId, utxo_60_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count(), 10);
        Assert.Equal(response.ChangeOutputs.Count(), 2);

        var selectedUTXOsSumAsset1 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);
        var outputsSumAsset1 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        var selectedUTXOsSumAsset2 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);
        var outputsSumAsset2 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        var changeOutputSumAsset1 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        var changeOutputSumAsset2 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        
        Assert.Equal(selectedUTXOsSumAsset1 + outputsSumAsset1 + changeOutputSumAsset1, 2);
        Assert.Equal(selectedUTXOsSumAsset2 + outputsSumAsset2 + changeOutputSumAsset2, 0);
    }

    [Fact]
    public void LargestFirst_MultiUtxo_MultiOutput_MultiMint_MultiBurn_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_burned_assets, output_10_ada_50_tokens, output_100_ada_no_assets, output_10_ada_1_already_minted_assets, output_10_ada_100_minted_assets, output_10_ada_1_minted_assets, output_10_ada_1_minted_assets, output_10_ada_2_burned_assets, output_10_ada_1_burned_assets };
        var utxos = new List<Utxo>()
        {
            utxo_50_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset_two,
            utxo_10_ada_1_owned_mint_asset,
            utxo_70_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset_two,
            utxo_10_ada_40_tokens,
            utxo_10_ada_20_tokens,
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_1_owned_mint_asset,
            utxo_60_ada_no_assets,
            utxo_10_ada_10_tokens,
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_30_tokens,
            utxo_10_ada_40_tokens,
            utxo_80_ada_no_assets,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[2].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[3].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[4].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[4].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[5].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[5].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[6].TxHash, utxo_10_ada_40_tokens.TxHash);
        Assert.Equal(response.Inputs[6].TransactionId, utxo_10_ada_40_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[7].TxHash, utxo_10_ada_40_tokens.TxHash);
        Assert.Equal(response.Inputs[7].TransactionId, utxo_10_ada_40_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[8].TxHash, utxo_80_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[8].TransactionId, utxo_80_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[9].TxHash, utxo_70_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[9].TransactionId, utxo_70_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count(), 10);
        Assert.Equal(response.ChangeOutputs.Count(), 2);

        var selectedUTXOsSumAsset1 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);
        var selectedUTXOsSumAsset2 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);
        var selectedUTXOsSumAsset3 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_100_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);
        var selectedUTXOsSumAsset4 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_40_tokens.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);

        var outputsSumAsset1 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        
        var outputsSumAsset2 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        var outputsSumAsset3 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_100_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_100_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        var outputsSumAsset4 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_40_tokens.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_40_tokens.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        var changeOutputSumAsset1 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        var changeOutputSumAsset2 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset_two.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        var changeOutputSumAsset3 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_100_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_100_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        var changeOutputSumAsset4 = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_40_tokens.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_40_tokens.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        
        Assert.Equal(selectedUTXOsSumAsset1, 4);
        Assert.Equal(outputsSumAsset1, 0);
        Assert.Equal(changeOutputSumAsset1, 0);

        Assert.Equal(selectedUTXOsSumAsset2, 2);
        Assert.Equal(outputsSumAsset2, -2);
        Assert.Equal(changeOutputSumAsset2, 0);

        Assert.Equal(selectedUTXOsSumAsset3, 0);
        Assert.Equal(outputsSumAsset3, 100);
        Assert.Equal(changeOutputSumAsset3, 0);

        Assert.Equal(selectedUTXOsSumAsset4, 80);
        Assert.Equal(outputsSumAsset4, 50);
        Assert.Equal(changeOutputSumAsset4, 30);
    }
}