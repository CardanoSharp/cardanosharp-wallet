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
    private TransactionOutput output_1_ada_no_assets;
    private TransactionOutput output_10_ada_no_assets;
    private TransactionOutput output_100_ada_no_assets;
    private TransactionOutput output_10_ada_50_tokens;
    private TransactionOutput output_10_ada_1_minted_assets;
    private TransactionOutput output_10_ada_100_minted_assets;
    private TransactionOutput output_10_ada_2_minted_assets;
    private TransactionOutput output_10_ada_200_minted_assets;
    private TransactionOutput output_10_ada_1_burned_assets;
    private TransactionOutput output_10_ada_100_burned_assets;
    private TransactionOutput output_10_ada_2_burned_assets;
    private TransactionOutput output_10_ada_200_burned_assets;
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

        output_10_ada_1_burned_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = burn_1_token_1_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Burn,
        };

        output_10_ada_100_burned_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = burn_1_token_100_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Burn,
        };

        output_10_ada_2_burned_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = burn_2_token_1_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Burn,
        };

        output_10_ada_200_burned_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 10 * lovelace,
                MultiAsset = burn_2_token_100_quantity.Build()
            },
            OutputPurpose = OutputPurpose.Burn,
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