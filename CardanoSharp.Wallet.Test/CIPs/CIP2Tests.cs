using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.CIPs.CIP2;
using CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;
using Xunit;

namespace CardanoSharp.Wallet.Test.CIPs;

public class CIP2Tests
{
    private TransactionOutput output_10_ada_no_assets;
    private TransactionOutput output_100_ada_no_assets;
    private TransactionOutput output_10_ada_50_tokens;

    private Asset asset_10_tokens;
    private Asset asset_20_tokens;
    private Asset asset_30_tokens;
    private Asset asset_40_tokens;
    private Asset asset_50_tokens;
    
    private Utxo utxo_10_ada_no_assets;
    private Utxo utxo_20_ada_no_assets;
    private Utxo utxo_30_ada_no_assets;
    private Utxo utxo_40_ada_no_assets;
    private Utxo utxo_50_ada_no_assets;
    private Utxo utxo_60_ada_no_assets;
    private Utxo utxo_70_ada_no_assets;
    private Utxo utxo_80_ada_no_assets;
    private Utxo utxo_90_ada_no_assets;
    private Utxo utxo_100_ada_no_assets;
    
    private Utxo utxo_10_ada_10_tokens;
    private Utxo utxo_10_ada_20_tokens;
    private Utxo utxo_10_ada_30_tokens;
    private Utxo utxo_10_ada_40_tokens;
    private Utxo utxo_10_ada_50_tokens;

    public CIP2Tests()
    {
        ulong lovelace = 1000000;
        string policyId = getRandomAssetPolicyIdHash();
        string assetName = getRandomAssetNameHash();
        
        utxo_10_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace
            }
        };
        utxo_20_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 20 * lovelace
            }
        };
        utxo_30_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 30 * lovelace
            }
        };
        utxo_40_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 40 * lovelace
            }
        };
        utxo_50_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 50 * lovelace
            }
        };
        utxo_60_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 60 * lovelace
            }
        };
        utxo_70_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 70 * lovelace
            }
        };
        utxo_80_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 80 * lovelace
            }
        };
        utxo_90_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 90 * lovelace
            }
        };
        utxo_100_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 100 * lovelace
            }
        };
        
        output_10_ada_no_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace
            }
        };
        
        output_100_ada_no_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 100 * lovelace
            }
        };
        
        output_10_ada_50_tokens = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = new Dictionary<byte[], NativeAsset>()
                {
                    {
                        policyId.HexToByteArray(), 
                        new NativeAsset()
                        {
                            Token = new Dictionary<byte[], ulong>()
                            {
                                { assetName.HexToByteArray(), 50 }
                            }
                        }
                    }
                }
            }
        };

        asset_10_tokens = new Asset()
        {
            PolicyId = policyId,
            Name = assetName,
            Quantity = 10
        };

        asset_20_tokens = new Asset()
        {
            PolicyId = policyId,
            Name = assetName,
            Quantity = 20
        };

        asset_30_tokens = new Asset()
        {
            PolicyId = policyId,
            Name = assetName,
            Quantity = 30
        };

        asset_40_tokens = new Asset()
        {
            PolicyId = policyId,
            Name = assetName,
            Quantity = 40
        };

        asset_50_tokens = new Asset()
        {
            PolicyId = policyId,
            Name = assetName,
            Quantity = 50
        };
        
        utxo_10_ada_10_tokens = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_10_tokens }
            }
        };
        
        utxo_10_ada_20_tokens = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_20_tokens }
            }
        };
        
        utxo_10_ada_30_tokens = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_30_tokens }
            }
        };
        
        utxo_10_ada_40_tokens = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_40_tokens }
            }
        };
        
        utxo_10_ada_50_tokens = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_50_tokens }
            }
        };
    }
    
    [Fact]
    public void LargestFirst_Simple_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), null);
        var outputs = new List<TransactionOutput>() { output_100_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets, 
            utxo_10_ada_no_assets, 
            utxo_30_ada_no_assets, 
            utxo_50_ada_no_assets
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_50_ada_no_assets.TxHash);
        Assert.Equal(response.SelectedUtxos[1].TxHash, utxo_40_ada_no_assets.TxHash);
        Assert.Equal(response.SelectedUtxos[2].TxHash, utxo_30_ada_no_assets.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_50_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.Inputs[1].TransactionId, utxo_40_ada_no_assets.TxHash.HexToByteArray());
        Assert.Equal(response.Inputs[2].TransactionId, utxo_30_ada_no_assets.TxHash.HexToByteArray());
    }
    
    [Fact]
    public void LargestFirst_SingleUtxo_Test()
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
    public void LargestFirst_LimitFail_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), null);
        var outputs = new List<TransactionOutput>() { output_100_ada_no_assets };
        var utxos = new List<Utxo>();
        for (var x = 0; x < 10; x++)
        {
            utxos.Add(utxo_10_ada_no_assets);
        }

        try
        {
            //act
            var response = coinSelection.GetCoinSelection(outputs, utxos, 5);
        }
        catch (Exception e)
        {
            //assert
            Assert.Equal("UTxOs have insufficient balance", e.Message);
        }
    }

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
    public void RandomImprove_LimitFail_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new RandomImproveStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_100_ada_no_assets };
        var utxos = new List<Utxo>();
        for (var x = 0; x < 10; x++)
        {
            utxos.Add(utxo_10_ada_no_assets);
        }

        try
        {
            //act
            var response = coinSelection.GetCoinSelection(outputs, utxos, 5);
        }
        catch (Exception e)
        {
            //assert
            Assert.Equal("UTxOs have insufficient balance", e.Message);
        }
    }

    [Fact]
    public void LargestFirst_WithTokens_Test()
    {
        //arrange 
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_50_tokens };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_40_tokens, 
            utxo_10_ada_10_tokens, 
            utxo_10_ada_30_tokens, 
            utxo_10_ada_50_tokens
        };
        
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
    public void LargestFirst_WithTokensAndAda_Test()
    {
        //arrange 
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_50_tokens, output_100_ada_no_assets };
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
    
    private string getRandomTransactionHash()
    {
        Random rnd = new Random();
        var hash = new byte[32];
        rnd.NextBytes(hash);
        return hash.ToStringHex();
    }
    
    private string getRandomAssetPolicyIdHash()
    {
        Random rnd = new Random();
        var hash = new byte[28];
        rnd.NextBytes(hash);
        return hash.ToStringHex();
    }
    
    private string getRandomAssetNameHash()
    {
        Random rnd = new Random();
        var hash = new byte[8];
        rnd.NextBytes(hash);
        return hash.ToStringHex();
    }
}