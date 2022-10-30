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
    public void RandomImprove_Simple_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new RandomImproveStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_100_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_no_assets, utxo_20_ada_no_assets, utxo_30_ada_no_assets, utxo_40_ada_no_assets,
            utxo_50_ada_no_assets,
            utxo_60_ada_no_assets, utxo_70_ada_no_assets, utxo_80_ada_no_assets, utxo_90_ada_no_assets,
            utxo_100_ada_no_assets
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        ulong totalSelected = 0;
        response.SelectedUtxos.ForEach(s => totalSelected = totalSelected + s.Balance.Lovelaces);
        ulong totalChange = 0;
        response.ChangeOutputs.ForEach(s => totalChange = totalChange + s.Value.Coin);
        Assert.True(totalSelected == totalChange + output_100_ada_no_assets.Value.Coin);
    }

    [Fact]
    public void RandomImprove_SingleUtxo_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_no_assets };
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
    public void RandomImprove_WithTokens_Test()
    {
        //arrange 
        var coinSelection = new CoinSelectionService(new RandomImproveStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_50_tokens };
        var utxos = new List<Utxo>();
        for (var x = 0; x < 10; x++)
        {
            utxos.Add(utxo_10_ada_20_tokens);
        }
        
        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);
        
        //assert
        //assert that selected utxo ada value is greater than the requested outputs' ada value
        Assert.True(response.SelectedUtxos.Sum(x => (long)x.Balance.Lovelaces) > outputs.Sum(x => (long)x.Value.Coin));
        
        //assert that selected utxo assets equal output + change asset values
        Assert.Equal(
            response.SelectedUtxos.Sum(x => 
                x.Balance.Assets.Where(y => 
                    y.PolicyId.Equals(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().PolicyId))
                        ?.Sum(z => (long)z.Quantity) ?? 0), 
            (response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Sum(y => 
                    y.Value.Token.Sum(z => (long)z.Value)) ?? 0)
                    +
                    outputs.Sum(x =>
                        x.Value.MultiAsset?.Sum(y => 
                            y.Value.Token.Sum(z => (long)z.Value)) ?? 0))
            );
        
        //assert that selected utxo ada value equal output + change utxo ada value
        Assert.Equal(response.SelectedUtxos.Sum(x => (long) x.Balance.Lovelaces), 
            (response.ChangeOutputs.Sum(x => (long)x.Value.Coin) 
                    +
                    outputs.Sum(x => (long)x.Value.Coin)));
    }

    [Fact]
    public void RandomImprove_WithTokensAndAda_Test()
    {
        //arrange 
        var coinSelection = new CoinSelectionService(new RandomImproveStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_50_tokens, output_100_ada_no_assets };
        var utxos = new List<Utxo>();
        for (var x = 0; x < 20; x++)
        {
            utxos.Add(utxo_10_ada_20_tokens);
        }
        
        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);
        
        //assert
        //assert that selected utxo ada value is greater than the requested outputs' ada value
        Assert.True(response.SelectedUtxos.Sum(x => (long)x.Balance.Lovelaces) > outputs.Sum(x => (long)x.Value.Coin));
        
        //assert that selected utxo assets equal output + change asset values
        Assert.Equal(
            response.SelectedUtxos.Sum(x => 
                x.Balance.Assets.Where(y => 
                    y.PolicyId.Equals(utxo_10_ada_50_tokens.Balance.Assets.FirstOrDefault().PolicyId))
                        ?.Sum(z => (long)z.Quantity) ?? 0), 
            (response.ChangeOutputs.Sum(x => 
                x.Value.MultiAsset?.Sum(y => 
                    y.Value.Token.Sum(z => (long)z.Value)) ?? 0)
                    +
                    outputs.Sum(x =>
                        x.Value.MultiAsset?.Sum(y => 
                            y.Value.Token.Sum(z => (long)z.Value)) ?? 0))
            );
        
        //assert that selected utxo ada value equal output + change utxo ada value
        Assert.Equal(response.SelectedUtxos.Sum(x => (long) x.Balance.Lovelaces), 
            (response.ChangeOutputs.Sum(x => (long)x.Value.Coin) 
                    +
                    outputs.Sum(x => (long)x.Value.Coin)));
    }
}