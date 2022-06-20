using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.CIPs.CIP2;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;
using Xunit;

namespace CardanoSharp.Wallet.Test.CIPs;

public class CIP2Tests
{
    
    [Fact]
    public void LargestFirst_Simple_Test()
    {
        //arrange
        var coinSelection = new CoinSelectionService(new LargestFirstStrategy(), null);
        var to1 = new TransactionOutput()
        {
            Address = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress().GetBytes(),
            Value = new TransactionOutputValue()
            {
                Coin = 100000000
            }
        };
        var outputs = new List<TransactionOutput>() {to1};
        
        
        var u1 = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 40000000
        };
        var u2 = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 10000000
        };
        var u3 = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 30000000
        };
        var u4 = new Utxo()
        {
            TxHash = getRandomTransactionHash(),
            TxIndex = 0,
            Value = 50000000
        };
        var utxos = new List<Utxo>() {u1, u2, u3, u4};

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.Equal(response.SelectedUtxos[0].TxHash, u4.TxHash);
        Assert.Equal(response.SelectedUtxos[1].TxHash, u1.TxHash);
        Assert.Equal(response.SelectedUtxos[2].TxHash, u3.TxHash);
        Assert.Equal(response.Inputs[0].TransactionId, u4.TxHash.HexToByteArray());
        Assert.Equal(response.Inputs[1].TransactionId, u1.TxHash.HexToByteArray());
        Assert.Equal(response.Inputs[2].TransactionId, u3.TxHash.HexToByteArray());
    }

    public void RandomImprove_Simple_Test()
    {
        //arrange
        
        //act
        
        //assert
    }
    
    private string getRandomTransactionHash()
    {
        Random rnd = new Random();
        var hash = new byte[32];
        rnd.NextBytes(hash);
        return hash.ToStringHex();
    }
}