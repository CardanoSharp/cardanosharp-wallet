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
    public void LargestFirst_SingleUTXO_SingleOutput_Mint_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_1_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_40_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.ChangeOutputs.Count() == 1);
    }
    [Fact]
    public void LargestFirst_MultiUTXO_SingleOutput_Mint_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_1_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_50_ada_no_assets
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_50_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_50_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.ChangeOutputs.Count() == 1);
    }

    [Fact]
    public void LargestFirst_SingleUTXO_MultiOutput_Mint_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_no_assets, output_10_ada_1_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_40_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.ChangeOutputs.Count() == 1);
    }

    [Fact]
    public void LargestFirst_MultiUTXO_MultiOutput_Mint_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_100_ada_no_assets, output_10_ada_1_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_10_ada_no_assets,
            utxo_50_ada_no_assets,
            utxo_20_ada_no_assets,
            utxo_30_ada_no_assets,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_50_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_50_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_40_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_30_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[2].TransactionId, utxo_30_ada_no_assets.TxHash.HexToByteArray());
    }

    [Fact]
    public void LargestFirst_WithTokens_Mint_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_50_tokens, output_10_ada_1_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_30_tokens, 
            utxo_10_ada_50_tokens,
            utxo_50_ada_no_assets,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_50_tokens.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_50_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_30_tokens.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_30_tokens.TxHash.HexToByteArray()); 
        Asset.Equals(response.SelectedUtxos.Count, 3);

        var selectedUTXOsSum = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
                x.Balance.Assets.Where(y => 
                    y.PolicyId.Equals(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().PolicyId))
                        ?.Sum(z => (long)z.Quantity) ?? 0);

        var changeOutputSum = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        
        var outputsSum = outputs.Sum(x =>
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
    
        Assert.Equal(selectedUTXOsSum, changeOutputSum + outputsSum);

        var mint = mint_1_token_1_quantity.Build();
        var mintQuantity = mint.Sum(x => x.Value.Token.Sum(z => (long)z.Value));
        Asset.Equals(mintQuantity, 1);

        // //assert that selected utxo ada value equal output + change utxo ada value
        Assert.Equal(response.SelectedUtxos.Sum(x => (long) x.Balance.Lovelaces), 
            (response.ChangeOutputs.Sum(x => (long)x.Value.Coin) 
                    +
                    outputs.Sum(x => (long)x.Value.Coin)));
    }

    [Fact]
    public void LargestFirst_SingleUTXO_SingleOutput_MultiMint_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_40_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Asset.Equals(response.ChangeOutputs.Count, 1);
    }

    [Fact]
    public void LargestFirst_MultiUTXO_SingleOutput_MultiMint_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_50_ada_no_assets
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_50_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_50_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.ChangeOutputs.Count() == 1);
    }

    [Fact]
    public void LargestFirst_SingleUTXO_MultiOutput_MultiMint_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets, output_10_ada_1_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_40_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.ChangeOutputs.Count() == 1);
    }

    [Fact]
    public void LargestFirst_MultiUTXO_MultiOutput_MultiMint_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_100_ada_no_assets, output_10_ada_50_tokens, output_10_ada_2_minted_assets, output_10_ada_1_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_50_ada_no_assets,
            utxo_10_ada_40_tokens,
            utxo_10_ada_30_tokens,
            utxo_60_ada_no_assets,
            utxo_90_ada_no_assets,
            utxo_10_ada_10_tokens,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_40_tokens.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_40_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_30_tokens.TxHash);
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_30_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_90_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[2].TransactionId, utxo_90_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_60_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[3].TransactionId, utxo_60_ada_no_assets.TxHash.HexToByteArray());
        Asset.Equals(response.SelectedUtxos.Count, 4);
        Assert.True(response.ChangeOutputs.Count() == 2);

        var selectedUTXOsSum = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
                x.Balance.Assets.Where(y => 
                    y.PolicyId.Equals(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().PolicyId))
                        ?.Sum(z => (long)z.Quantity) ?? 0);

        var changeOutputSum = response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
        
        var outputsSum = outputs.Sum(x =>
                x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                    y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);
    
        Assert.Equal(selectedUTXOsSum, changeOutputSum + outputsSum);

        var mintOne = mint_2_token_1_quantity.Build();
        var mintQuantityOne = mintOne.Sum(x => x.Value.Token.Sum(z => (long)z.Value));
        Asset.Equals(mintQuantityOne, 2);

        var mintTwo = mint_2_token_1_quantity.Build();
        var mintQuantityTwo = mintTwo.Sum(x => x.Value.Token.Sum(z => (long)z.Value));
        Asset.Equals(mintQuantityTwo, 2);

        // //assert that selected utxo ada value equal output + change utxo ada value
        Assert.Equal(response.SelectedUtxos.Sum(x => (long) x.Balance.Lovelaces), 
            (response.ChangeOutputs.Sum(x => (long)x.Value.Coin) 
                    +
                    outputs.Sum(x => (long)x.Value.Coin)));
    }

    [Fact]
    public void LargestFirst_MintAndOwned_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_1_minted_assets, output_10_ada_1_minted_assets, output_10_ada_1_already_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);        
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_40_ada_no_assets.TxHash);        
        Assert.Equal(response.Inputs[1].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.SelectedUtxos.Count() == 2);
        Assert.True(response.ChangeOutputs.Count() == 1);
    }

    [Fact]
    public void LargestFirst_MintAndOwned_Test_2()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets, output_10_ada_1_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_40_ada_no_assets.TxHash);        
        Assert.Equal(response.Inputs[0].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.SelectedUtxos.Count() == 1);
        Assert.True(response.ChangeOutputs.Count() == 1);
    }

    [Fact]
    public void LargestFirst_MintAndOwned_Test_3()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets, output_10_ada_1_minted_assets, output_10_ada_1_already_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);        
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_40_ada_no_assets.TxHash);        
        Assert.Equal(response.Inputs[1].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.SelectedUtxos.Count() == 2);
        Assert.True(response.ChangeOutputs.Count() == 1);
    }

    [Fact]
    public void LargestFirst_MintAndOwned_Test_4()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets, output_10_ada_1_minted_assets, output_10_ada_2_already_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_1_owned_mint_asset_two,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);        
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());        
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);        
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_40_ada_no_assets.TxHash);        
        Assert.Equal(response.Inputs[2].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.SelectedUtxos.Count() == 3);
        Assert.True(response.ChangeOutputs.Count() == 1);
    }

    [Fact]
    public void LargestFirst_MintAndOwned_Test_5()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets, output_10_ada_1_minted_assets, output_10_ada_2_already_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_100_owned_mint_asset,
            utxo_10_ada_40_tokens,
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_1_owned_mint_asset_two,
            utxo_10_ada_50_tokens,
            utxo_10_ada_30_tokens
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);        
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());        
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_1_owned_mint_asset_two.TxHash);        
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_1_owned_mint_asset_two.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_10_ada_100_owned_mint_asset.TxHash);        
        Assert.Equal(response.Inputs[2].TransactionId, utxo_10_ada_100_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_10_ada_40_tokens.TxHash);        
        Assert.Equal(response.Inputs[3].TransactionId, utxo_10_ada_40_tokens.TxHash.HexToByteArray());
        Assert.True(response.SelectedUtxos.Count() == 4);
        Assert.True(response.ChangeOutputs.Count() == 1);
    }

    [Fact]
    public void LargestFirst_MintAndOwned_Test_Fail()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets, output_10_ada_1_minted_assets, output_10_ada_1_already_minted_assets, output_10_ada_1_already_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset,
        };

        try
        {
            //act            
            var response = coinSelection.GetCoinSelection(outputs, utxos);
        }
        catch (Exception e)
        {
            //assert
            Assert.Equal("UTxOs have insufficient balance", e.Message);
        }
    }

    [Fact]
    public void LargestFirst_MultiOption_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets, output_10_ada_1_minted_assets, output_100_ada_no_assets, output_10_ada_50_tokens };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_100_owned_mint_asset,
            utxo_10_ada_40_tokens,
            utxo_60_ada_no_assets,            
            utxo_10_ada_20_tokens,            
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_40_tokens.TxHash);        
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_40_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_20_tokens.TxHash);        
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_20_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_60_ada_no_assets.TxHash);        
        Assert.Equal(response.Inputs[2].TransactionId, utxo_60_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_40_ada_no_assets.TxHash);        
        Assert.Equal(response.Inputs[3].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[4].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);        
        Assert.Equal(response.Inputs[4].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[5].TxHash, utxo_10_ada_100_owned_mint_asset.TxHash);        
        Assert.Equal(response.Inputs[5].TransactionId, utxo_10_ada_100_owned_mint_asset.TxHash.HexToByteArray());
        Assert.True(response.SelectedUtxos.Count() == 6);
        Assert.True(response.ChangeOutputs.Count() == 2);
    }

    [Fact]
    public void LargestFirst_MultiOption_Test_2()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_2_minted_assets, output_10_ada_1_minted_assets, output_10_ada_1_already_minted_assets, output_100_ada_no_assets, output_10_ada_50_tokens };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
            utxo_10_ada_1_owned_mint_asset,
            utxo_10_ada_100_owned_mint_asset,
            utxo_10_ada_40_tokens,
            utxo_60_ada_no_assets,            
            utxo_10_ada_20_tokens, 
            utxo_90_ada_no_assets,           
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);        
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_10_ada_40_tokens.TxHash);        
        Assert.Equal(response.Inputs[1].TransactionId, utxo_10_ada_40_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_10_ada_20_tokens.TxHash);        
        Assert.Equal(response.Inputs[2].TransactionId, utxo_10_ada_20_tokens.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[3].TxHash, utxo_90_ada_no_assets.TxHash);        
        Assert.Equal(response.Inputs[3].TransactionId, utxo_90_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos[4].TxHash, utxo_60_ada_no_assets.TxHash);        
        Assert.Equal(response.Inputs[4].TransactionId, utxo_60_ada_no_assets.TxHash.HexToByteArray());
        Assert.True(response.SelectedUtxos.Count() == 5);
        Assert.True(response.ChangeOutputs.Count() == 2);
    }
}