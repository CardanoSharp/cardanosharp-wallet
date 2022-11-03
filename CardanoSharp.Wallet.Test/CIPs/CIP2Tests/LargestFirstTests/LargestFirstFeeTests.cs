using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2;
using CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using Xunit;

namespace CardanoSharp.Wallet.Test.CIPs;

public partial class CIP2Tests
{
    [Fact]
    public void LargestFirst_Simple_Fee_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_100_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_50_ada_no_assets, 
            utxo_50_ada_no_assets,
            utxo_10_ada_no_assets,
            utxo_20_ada_no_assets,
        };

        //act
        ulong feeBuffer = 21 * adaToLovelace;
        var response = coinSelection.GetCoinSelection(outputs, utxos, address, feeBuffer: feeBuffer);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_50_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_50_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_50_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_50_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_20_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[2].TransactionId, utxo_20_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_10_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[3].TransactionId, utxo_10_ada_no_assets.TxHash.HexToByteArray());
        
        long totalSelected = 0;
        response.SelectedUtxos.ForEach(s => totalSelected = totalSelected + (long)s.Balance.Lovelaces);
        long totalOutput = 0;
        outputs.ForEach(o => totalOutput = totalOutput + (long)o.Value.Coin);
        long totalChange = 0;
        response.ChangeOutputs.ForEach(s => totalChange = totalChange + (long)s.Value.Coin);
        long finalChangeOutputChange = (long)response.ChangeOutputs.Last().Value.Coin;
        Assert.Equal(totalSelected, totalOutput + totalChange);
        Assert.True((ulong)finalChangeOutputChange >= feeBuffer);
    }

    [Fact]
    public void LargestFirst_Simple_Fee_Fail_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_100_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_50_ada_no_assets, 
            utxo_50_ada_no_assets,
            utxo_10_ada_no_assets,
        };

        //assert
        try
        {
            //act
            var response = coinSelection.GetCoinSelection(outputs, utxos, address, feeBuffer: 11 * adaToLovelace);
        }
        catch (Exception e)
        {
            //assert
            Assert.Equal("UTxOs have insufficient balance", e.Message);
        }
    }
    
    [Fact]
    public void LargestFirst_BasicChange_Fee_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new BasicChangeSelectionStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_no_assets, output_10_ada_no_assets, output_10_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_10_tokens, 
            utxo_10_ada_1_owned_mint_asset, 
            utxo_10_ada_1_owned_mint_asset_two, 
            utxo_10_ada_100_owned_mint_asset,
        };

        //act
        ulong feeBuffer = 3 * adaToLovelace;
        var response = coinSelection.GetCoinSelection(outputs, utxos, address, feeBuffer: feeBuffer);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_10_tokens.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_10_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[2].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_10_ada_100_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[3].TransactionId, utxo_10_ada_100_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count, 4);
        Assert.Equal(response.ChangeOutputs.Count, 1);

        long totalSelected = 0;
        response.SelectedUtxos.ForEach(s => totalSelected = totalSelected + (long)s.Balance.Lovelaces);
        long totalOutput = 0;
        outputs.ForEach(o => totalOutput = totalOutput + (long)o.Value.Coin);
        long totalChange = 0;
        response.ChangeOutputs.ForEach(s => totalChange = totalChange + (long)s.Value.Coin);
        long finalChangeOutputChange = (long)response.ChangeOutputs.Last().Value.Coin;
        Assert.Equal(totalSelected, totalOutput + totalChange);
        Assert.True((ulong)finalChangeOutputChange >= feeBuffer);
    }

    [Fact]
    public void LargestFirst_BasicChange_Fee_Test_2()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new BasicChangeSelectionStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_no_assets, output_10_ada_no_assets, output_10_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_10_tokens, 
            utxo_10_ada_1_owned_mint_asset, 
            utxo_10_ada_1_owned_mint_asset_two, 
            utxo_10_ada_100_owned_mint_asset,
            utxo_10_ada_100_owned_mint_asset_two,
        };

        //act
        ulong feeBuffer = 11 * adaToLovelace;
        var response = coinSelection.GetCoinSelection(outputs, utxos, address, feeBuffer: 11 * adaToLovelace);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_10_tokens.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_10_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[2].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_10_ada_100_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[3].TransactionId, utxo_10_ada_100_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[4].TxHash, utxo_10_ada_100_owned_mint_asset_two.TxHash);
        Assert.Equal(response.Inputs[4].TransactionId, utxo_10_ada_100_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count, 5);
        Assert.Equal(response.ChangeOutputs.Count, 1);
        Assert.Equal(response.ChangeOutputs.First().Value.MultiAsset.Count, 5);

        long totalSelected = 0;
        response.SelectedUtxos.ForEach(s => totalSelected = totalSelected + (long)s.Balance.Lovelaces);
        long totalOutput = 0;
        outputs.ForEach(o => totalOutput = totalOutput + (long)o.Value.Coin);
        long totalChange = 0;
        response.ChangeOutputs.ForEach(s => totalChange = totalChange + (long)s.Value.Coin);
        long finalChangeOutputChange = (long)response.ChangeOutputs.Last().Value.Coin;
        Assert.Equal(totalSelected, totalOutput + totalChange);
        Assert.True((ulong)finalChangeOutputChange >= feeBuffer);
    }

    [Fact]
    public void LargestFirst_BasicChange_Fee_Fail_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new BasicChangeSelectionStrategy());
        var outputs = new List<TransactionOutput>() { output_100_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_50_ada_no_assets, 
            utxo_50_ada_no_assets,
            utxo_10_ada_no_assets,
        };

        //assert
        try
        {
            //act
            var response = coinSelection.GetCoinSelection(outputs, utxos, address, feeBuffer: 11 * adaToLovelace);
        }
        catch (Exception e)
        {
            //assert
            Assert.Equal("UTxOs have insufficient balance", e.Message);
        }
    }
}