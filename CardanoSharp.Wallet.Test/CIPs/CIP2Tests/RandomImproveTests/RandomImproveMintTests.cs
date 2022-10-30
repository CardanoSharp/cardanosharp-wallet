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
    public void RandomImprove_SingleUTXO_SingleOutput_Mint_Test()
    {
        var coinSelection = new CoinSelectionService(new RandomImproveStrategy(), new SingleTokenBundleStrategy());
        var outputs = new List<TransactionOutput>() { output_10_ada_1_minted_assets };
        var utxos = new List<Utxo>()
        {
            utxo_40_ada_no_assets,
        };

        //act
        var response = coinSelection.GetCoinSelection(outputs, utxos);

        //assert
        Assert.True(response.SelectedUtxos.Count() == 1);
        Assert.True(response.ChangeOutputs.Count() == 1);

        //assert
        long totalSelected = 0;
        response.SelectedUtxos.ForEach(s => totalSelected = totalSelected + (long)s.Balance.Lovelaces);
        long totalOutput = 0;
        outputs.ForEach(o => totalOutput = totalOutput + (long)o.Value.Coin);
        long totalChange = 0;
        response.ChangeOutputs.ForEach(s => totalChange = totalChange + (long)s.Value.Coin);

        var selectedUTXOsSumAsset1 = response.SelectedUtxos.Where(x => x.Balance.Assets is not null).Sum(x => 
            x.Balance.Assets.Where(y => 
                y.PolicyId.Equals(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId))
                    ?.Sum(z => (long)z.Quantity) ?? 0);

        var outputsSumAsset1 = outputs.Sum(x =>
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        var changeOutputSumAsset1 = response.ChangeOutputs.Sum(x => 
            x.Value.MultiAsset?.Where(y => y.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().PolicyId)).Sum(y => 
                y.Value.Token.Where(z => z.Key.ToStringHex().SequenceEqual(utxo_10_ada_1_owned_mint_asset.Balance.Assets.FirstOrDefault().Name)).Sum(z => (long)z.Value)) ?? 0);

        Assert.Equal(totalSelected, 40000000);
        Assert.Equal(totalOutput, 10000000);
        Assert.Equal(totalChange, 30000000);

        Assert.Equal(selectedUTXOsSumAsset1, 0);
        Assert.Equal(outputsSumAsset1, 1);
        Assert.Equal(changeOutputSumAsset1, 0);
    }
}