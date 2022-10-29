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

public class CIP2Tests
{
    private TransactionOutput output_1_ada_no_assets;
    private TransactionOutput output_10_ada_no_assets;
    private TransactionOutput output_100_ada_no_assets;
    private TransactionOutput output_10_ada_50_tokens;
    private TransactionOutput output_10_ada_1_minted_assets;
    private TransactionOutput output_10_ada_100_minted_assets;
    private TransactionOutput output_10_ada_2_minted_assets;
    private TransactionOutput output_10_ada_200_minted_assets;
    private TransactionOutput output_10_ada_1_already_minted_assets;
    private TransactionOutput output_10_ada_100_already_minted_assets;
    private TransactionOutput output_10_ada_2_already_minted_assets;
    private TransactionOutput output_10_ada_200_already_minted_assets;
    private TransactionOutput output_10_ada_50_tokens_1_minted_assets;
    private TransactionOutput output_10_ada_50_tokens_100_minted_assets;
    private TransactionOutput output_10_ada_50_tokens_1_already_minted_assets;
    private TransactionOutput output_10_ada_50_tokens_100_already_minted_assets;
    private TransactionOutput output_10_ada_50_tokens_1_minted_assets_1_already_minted_assets;
    private TransactionOutput output_10_ada_50_tokens_1_minted_assets_100_already_minted_assets;

    private Asset asset_10_tokens;
    private Asset asset_20_tokens;
    private Asset asset_30_tokens;
    private Asset asset_40_tokens;
    private Asset asset_50_tokens;
    private Asset asset_mint_1_tokens_1;
    private Asset asset_mint_1_tokens_100;
    private Asset asset_mint_2_tokens_1;
    private Asset asset_mint_2_tokens_100;
    private Asset asset_mint_1_tokens_2;
    
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

    private Utxo utxo_10_ada_1_owned_mint_asset;
    private Utxo utxo_10_ada_1_owned_mint_asset_two;
    private Utxo utxo_10_ada_100_owned_mint_asset;
    private Utxo utxo_10_ada_100_owned_mint_asset_two;
    private Utxo utxo_10_ada_3_owned_mint_asset;

    private ITokenBundleBuilder mint_1_token_1_quantity;
    private ITokenBundleBuilder mint_1_token_100_quantity;
    private ITokenBundleBuilder mint_2_token_1_quantity;
    private ITokenBundleBuilder mint_2_token_100_quantity;
    private ITokenBundleBuilder mint_3_token_1_quantity;

    private ITokenBundleBuilder burn_1_token_1_quantity;
    private ITokenBundleBuilder burn_1_token_100_quantity;
    private ITokenBundleBuilder burn_2_token_1_quantity;
    private ITokenBundleBuilder burn_2_token_100_quantity;
    private ITokenBundleBuilder burn_3_token_1_quantity;

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

        output_1_ada_no_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 1 * lovelace
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
                            Token = new Dictionary<byte[], long>()
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
        
        var mint_policy_1 = "4a4c17cc89b90f7239ce83f41e4f47005859870178f4e6815b1cd318";
        var mint_policy_1_asset_1 = "ADABlob1";
        var mint_policy_2 = "4a4c17cc89b90f7239ce83f41e4f47005859870178f4e6815b1cd317";
        var mint_policy_2_asset_1 = "SpaceBlob1";
        var mint_policy_3 = "d5e6bf0500378d4f0da4e8dde6becec7621cd8cbf5cbb9b87013d4cc";
        var mint_policy_3_asset_1 = "ADABlobs1";
        var mint_policy_4 = "d5e6bf0500378d4f0da4e8dde6becec7621cd8cbf5cbb9b87013d4cb";
        var mint_policy_4_asset_1 = "SpaceBlobs1";

        mint_1_token_1_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        mint_1_token_1_quantity.AddToken(mint_policy_1.HexToByteArray(), mint_policy_1_asset_1.ToBytes(), 1);

        mint_1_token_100_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        mint_1_token_100_quantity.AddToken(mint_policy_3.HexToByteArray(), mint_policy_3_asset_1.ToBytes(), 100);

        mint_2_token_1_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        mint_2_token_1_quantity.AddToken(mint_policy_1.HexToByteArray(), mint_policy_1_asset_1.ToBytes(), 1);
        mint_2_token_1_quantity.AddToken(mint_policy_2.HexToByteArray(), mint_policy_2_asset_1.ToBytes(), 1);

        mint_2_token_100_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        mint_2_token_100_quantity.AddToken(mint_policy_3.HexToByteArray(), mint_policy_3_asset_1.ToBytes(), 100);
        mint_2_token_100_quantity.AddToken(mint_policy_4.HexToByteArray(), mint_policy_4_asset_1.ToBytes(), 100);

        mint_3_token_1_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        mint_3_token_1_quantity.AddToken(mint_policy_1.HexToByteArray(), mint_policy_1_asset_1.ToBytes(), 2);
        mint_3_token_1_quantity.AddToken(mint_policy_2.HexToByteArray(), mint_policy_2_asset_1.ToBytes(), 1);

        burn_1_token_1_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        burn_1_token_1_quantity.AddToken(mint_policy_1.HexToByteArray(), mint_policy_1_asset_1.ToBytes(), -1);

        burn_1_token_100_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        burn_1_token_100_quantity.AddToken(mint_policy_3.HexToByteArray(), mint_policy_3_asset_1.ToBytes(), -100);

        burn_2_token_1_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        burn_2_token_1_quantity.AddToken(mint_policy_1.HexToByteArray(), mint_policy_1_asset_1.ToBytes(), -1);
        burn_2_token_1_quantity.AddToken(mint_policy_2.HexToByteArray(), mint_policy_2_asset_1.ToBytes(), -1);

        burn_2_token_100_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        burn_2_token_100_quantity.AddToken(mint_policy_3.HexToByteArray(), mint_policy_3_asset_1.ToBytes(), -100);
        burn_2_token_100_quantity.AddToken(mint_policy_4.HexToByteArray(), mint_policy_4_asset_1.ToBytes(), -100);

        burn_3_token_1_quantity = (ITokenBundleBuilder)TokenBundleBuilder.Create;
        burn_3_token_1_quantity.AddToken(mint_policy_1.HexToByteArray(), mint_policy_1_asset_1.ToBytes(), -2);
        burn_3_token_1_quantity.AddToken(mint_policy_2.HexToByteArray(), mint_policy_2_asset_1.ToBytes(), -1);

        asset_mint_1_tokens_1 = new Asset()
        {
            PolicyId = mint_policy_1,
            Name = mint_policy_1_asset_1.ToBytes().ToStringHex(),
            Quantity = 1
        };

        asset_mint_1_tokens_100 = new Asset()
        {
            PolicyId = mint_policy_3,
            Name = mint_policy_3_asset_1.ToBytes().ToStringHex(),
            Quantity = 100
        };

        asset_mint_2_tokens_1 = new Asset()
        {
            PolicyId = mint_policy_2,
            Name = mint_policy_2_asset_1.ToBytes().ToStringHex(),
            Quantity = 1
        };

        asset_mint_2_tokens_100 = new Asset()
        {
            PolicyId = mint_policy_4,
            Name = mint_policy_4_asset_1.ToBytes().ToStringHex(),
            Quantity = 100
        };

        asset_mint_1_tokens_2 = new Asset()
        {
            PolicyId = mint_policy_1,
            Name = mint_policy_1_asset_1.ToBytes().ToStringHex(),
            Quantity = 2
        };

        utxo_10_ada_1_owned_mint_asset = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_mint_1_tokens_1 }
            }
        };

        utxo_10_ada_100_owned_mint_asset = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_mint_1_tokens_100 }
            }
        };

        utxo_10_ada_1_owned_mint_asset_two = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_mint_2_tokens_1 }
            }
        };

        utxo_10_ada_100_owned_mint_asset_two = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_mint_2_tokens_100 }
            }
        };

        utxo_10_ada_3_owned_mint_asset = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Balance = new Balance()
            {
                Lovelaces = 10 * lovelace,
                Assets = new List<Asset>() { asset_mint_2_tokens_100 }
            }
        };

        
        output_10_ada_1_minted_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = mint_1_token_1_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Mint,
        };

        output_10_ada_100_minted_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = mint_1_token_100_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Mint,
        };

        output_10_ada_2_minted_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = mint_2_token_1_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Mint,
        };

        output_10_ada_200_minted_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = mint_2_token_100_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Mint,
        };

         output_10_ada_1_already_minted_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = mint_1_token_1_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Spend,
        };

        output_10_ada_100_already_minted_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = mint_1_token_100_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Spend,
        };

        output_10_ada_2_already_minted_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = mint_2_token_1_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Spend,
        };

        output_10_ada_200_already_minted_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = mint_2_token_100_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Spend,
        };

        output_10_ada_50_tokens_1_minted_assets = new TransactionOutput()
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
                            Token = new Dictionary<byte[], long>()
                            {
                                { assetName.HexToByteArray(), 50 }
                            }
                        }
                    },
                    {
                        mint_1_token_1_quantity.Build().FirstOrDefault().Key,
                        mint_1_token_1_quantity.Build().FirstOrDefault().Value
                    }
                } 
            },
            OutputPurpose = OutputPurpose.Mint,
        };

        output_10_ada_50_tokens_100_minted_assets = new TransactionOutput()
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
                            Token = new Dictionary<byte[], long>()
                            {
                                { assetName.HexToByteArray(), 50 }
                            }
                        }
                    },
                    {
                        mint_1_token_100_quantity.Build().FirstOrDefault().Key,
                        mint_1_token_100_quantity.Build().FirstOrDefault().Value
                    }
                } 
            },
            OutputPurpose = OutputPurpose.Mint,
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

    [Fact]
    public void LargestFirst_SingleUTXO_SingleOutput_Burn_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_1_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_1_owned_mint_asset
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count(), 1);
        Assert.Equal(response.ChangeOutputs.Count(), 1);

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
    
        var burn = burn_1_token_1_quantity.Build();
        var burnQuantity = burn.Sum(x => x.Value.Token.Sum(z => (long)z.Value));
        Asset.Equals(burnQuantity, -1);

        Assert.Equal(selectedUTXOsSum + burnQuantity, changeOutputSum + outputsSum);
    }

    /*
    [Fact]
    public void LargestFirst_SingleUTXO_MultiOutput_Burn_Test()
    {
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_1_ada_no_assets, output_1_ada_no_assets, output_1_ada_no_assets };
        var utxos = new List<Utxo>()
        {
            utxo_10_ada_1_owned_mint_asset
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos, mint: (TokenBundleBuilder)burn_1_token_1_quantity);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, utxo_10_ada_1_owned_mint_asset.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, utxo_10_ada_1_owned_mint_asset.TxHash.HexToByteArray());
        Assert.Equal(response.SelectedUtxos.Count(), 1);
        Assert.Equal(response.ChangeOutputs.Count(), 1);

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
    
        var burn = burn_1_token_1_quantity.Build();
        var burnQuantity = burn.Sum(x => x.Value.Token.Sum(z => (long)z.Value));
        Asset.Equals(burnQuantity, -1);

        Assert.Equal(selectedUTXOsSum + burnQuantity, changeOutputSum + outputsSum);
    }
    */
    
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