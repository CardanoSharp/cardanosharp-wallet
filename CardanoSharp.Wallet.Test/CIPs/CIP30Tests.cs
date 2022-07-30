using CardanoSharp.Wallet.CIPs.CIP30.Extensions.Models;
using CardanoSharp.Wallet.CIPs.CIP30.Models;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CardanoSharp.Wallet.Test.CIPs;

public class CIP30Tests
{
	[Fact]
	public void TransactionUnspentOutput_Serialization_Test()
	{
		TransactionUnspentOutput unspentOutput = new TransactionUnspentOutput()
		{
			Input = new TransactionInput()
			{
				TransactionId = "E1DBF61F88853BB413EBD4681A5D0DCF7E6D48785463F348B9739CCC4F7D3069".HexToByteArray(),
				TransactionIndex = 1
			},
			Output = new TransactionOutput()
			{
				Address = "005A99CB175EB944462D6BFD29D06E0A69DEFC091D8E5ECAB740AFAC6F1922FCDEEB6DF8592D78B20C6E22FDB73FA9446AAD05626D78000B7F".HexToByteArray(),
				Value = new TransactionOutputValue()
				{
					Coin = 1551690,
					MultiAsset = new Dictionary<byte[], NativeAsset>()
					{
						{
							"698A6EA0CA99F315034072AF31EAAC6EC11FE8558D3F48E9775AAB9D".HexToByteArray(),
							new NativeAsset()
							{
								Token = new Dictionary<byte[], long>()
								{
									{ "7444524950".HexToByteArray(), 3999999899 }
								}
							}
						},
						{
							"A4A461B1F5C751D4EFDFF99070CA9FAED2293DE3578AC7401F0ED5CC".HexToByteArray(),
							new NativeAsset()
							{
								Token = new Dictionary<byte[], long>()
								{
									{ "744C616365".HexToByteArray(), 3 }
								}
							}
						}
					}
				}
			}
		};
		var actualCbor = unspentOutput.Serialize().ToStringHex();
		var expectedCbor = "82825820e1dbf61f88853bb413ebd4681a5d0dcf7e6d48785463f348b9739ccc4f7d306901825839005a99cb175eb944462d6bfd29d06e0a69defc091d8e5ecab740afac6f1922fcdeeb6df8592d78b20c6e22fdb73fa9446aad05626d78000b7f821a0017ad4aa2581c698a6ea0ca99f315034072af31eaac6ec11fe8558d3f48e9775aab9da14574445249501aee6b279b581ca4a461b1f5c751d4efdff99070ca9faed2293de3578ac7401f0ed5cca145744c61636503";
		Assert.Equal(expectedCbor, actualCbor);
	}

	[Fact]
	public void TransactionUnspentOutput_Deserialization_Test()
	{
		var transactionUnspentOutputCbor = "82825820480296351080921cea6f56d56f87390bc698a6e1f2b3fe105dee1978780a382301825839005a99cb175eb944462d6bfd29d06e0a69defc091d8e5ecab740afac6f1922fcdeeb6df8592d78b20c6e22fdb73fa9446aad05626d78000b7f821a0014851ea1581c635da8872ab583e67993c69e67f50f12cc34ef8e1e1d93da9a9fe0cda144544d4f4e191f40";
		var unspentOutput = transactionUnspentOutputCbor.HexToByteArray().DeserializeTransactionUnspentOutput();
		Assert.Equal("480296351080921CEA6F56D56F87390BC698A6E1F2B3FE105DEE1978780A3823",
			unspentOutput.Input.TransactionId.ToStringHex().ToUpper());
		Assert.Equal((uint)1, unspentOutput.Input.TransactionIndex);
		Assert.Equal("005A99CB175EB944462D6BFD29D06E0A69DEFC091D8E5ECAB740AFAC6F1922FCDEEB6DF8592D78B20C6E22FDB73FA9446AAD05626D78000B7F",
			unspentOutput.Output.Address.ToStringHex().ToUpper());
		Assert.Equal(1344798, unspentOutput.Output.Value.Coin);
		var multiAsset = unspentOutput.Output.Value.MultiAsset;
		Assert.NotNull(multiAsset);
		Assert.Single(multiAsset);
		Assert.Equal("635DA8872AB583E67993C69E67F50F12CC34EF8E1E1D93DA9A9FE0CD",
			multiAsset.Keys.First().ToStringHex().ToUpper());
		var token = multiAsset.Values.First().Token;
		Assert.NotNull(token);
		Assert.Single(token);
		Assert.Equal("544D4F4E",
			token.Keys.First().ToStringHex().ToUpper());
		Assert.Equal((long)8000, token.Values.First());
	}

	[Fact]
	public void Utxo_Serialization_Test()
	{
		Utxo utxo = new Utxo()
		{
			TxHash = "4d1abeeb92a43b6504c8e73052e327e960c1a7dc8bb2b7a0b68fda925f74f576",
			TxIndex = 0,
			OutputAddress = "addr_test1qpdfnjcht6u5g33dd07jn5rwpf5aalqfrk89aj4hgzh6cmceyt7da6mdlpvj679jp3hz9ldh8755g64dq43x67qqpdls6uzr2z",
			Balance = new Balance()
			{
				Lovelaces = 1344798,
				Assets = new List<Asset>()
				{
					new Asset()
					{
						PolicyId = "635da8872ab583e67993c69e67f50f12cc34ef8e1e1d93da9a9fe0cd",
						Name = "TMON",
						Quantity = 2000
					}
				}
			}
		};
		var actualCbor = utxo.Serialize().ToStringHex();
		var expectedCbor = "828258204d1abeeb92a43b6504c8e73052e327e960c1a7dc8bb2b7a0b68fda925f74f57600825839005a99cb175eb944462d6bfd29d06e0a69defc091d8e5ecab740afac6f1922fcdeeb6df8592d78b20c6e22fdb73fa9446aad05626d78000b7f821a0014851ea1581c635da8872ab583e67993c69e67f50f12cc34ef8e1e1d93da9a9fe0cda144544d4f4e1907d0";
		Assert.Equal(expectedCbor, actualCbor);
	}

	[Fact]
	public void Utxo_Deserialization_Test()
	{
		var utxoCbor = "828258207b2eb1b7126b5a94a5fb8dafee73c54210a3fe220a6d3b0effc7278471be185d00825839005a99cb175eb944462d6bfd29d06e0a69defc091d8e5ecab740afac6f1922fcdeeb6df8592d78b20c6e22fdb73fa9446aad05626d78000b7f821a0014851ea1581c635da8872ab583e67993c69e67f50f12cc34ef8e1e1d93da9a9fe0cda144544d4f4e191770";
		var utxo = utxoCbor.HexToByteArray().DeserializeUtxo();
		Assert.Equal("7b2eb1b7126b5a94a5fb8dafee73c54210a3fe220a6d3b0effc7278471be185d",
			utxo.TxHash);
		Assert.Equal((uint)0, utxo.TxIndex);
		Assert.Equal("addr_test1qpdfnjcht6u5g33dd07jn5rwpf5aalqfrk89aj4hgzh6cmceyt7da6mdlpvj679jp3hz9ldh8755g64dq43x67qqpdls6uzr2z",
			utxo.OutputAddress);
		Assert.Equal(1344798, utxo.Balance.Lovelaces);
		Assert.NotNull(utxo.Balance.Assets);
		Assert.Equal(1, utxo.Balance.Assets.Count);
		Assert.Equal("635da8872ab583e67993c69e67f50f12cc34ef8e1e1d93da9a9fe0cd", utxo.Balance.Assets.First().PolicyId);
		Assert.Equal("TMON", utxo.Balance.Assets.First().Name);
		Assert.Equal((long)6000, utxo.Balance.Assets.First().Quantity);
	}
}