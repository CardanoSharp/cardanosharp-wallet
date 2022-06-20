using System;
using System.Collections.Generic;
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
    private TransactionOutput input_100_ada_no_assets;
    
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

    public CIP2Tests()
    {
        ulong lovelace = 1000000;
        utxo_10_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 10 * lovelace
        };
        utxo_20_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 20 * lovelace
        };
        utxo_30_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 30 * lovelace
        };
        utxo_40_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 40 * lovelace
        };
        utxo_50_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 50 * lovelace
        };
        utxo_60_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 60 * lovelace
        };
        utxo_70_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 70 * lovelace
        };
        utxo_80_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 80 * lovelace
        };
        utxo_90_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 90 * lovelace
        };
        utxo_100_ada_no_assets = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 100 * lovelace
        };
        
        input_100_ada_no_assets = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 100000000
            }
        };
    }
    
    [Fact]
    public void LargestFirst_Simple_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), null);
        var outputs = new List<TransactionOutput>() { input_100_ada_no_assets };
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
    public void LargestFirst_LimitFail_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), null);
        var outputs = new List<TransactionOutput>() { input_100_ada_no_assets };
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
        var outputs = new List<TransactionOutput>() { input_100_ada_no_assets };
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
        response.SelectedUtxos.ForEach(s => totalSelected = totalSelected + s.Value);
        ulong totalChange = 0;
        response.ChangeOutputs.ForEach(s => totalChange = totalChange + s.Value.Coin);
        Assert.True(totalSelected == totalChange + input_100_ada_no_assets.Value.Coin);
    }
    
    private string getRandomTransactionHash()
    {
        Random rnd = new Random();
        var hash = new byte[32];
        rnd.NextBytes(hash);
        return hash.ToStringHex();
    }
}