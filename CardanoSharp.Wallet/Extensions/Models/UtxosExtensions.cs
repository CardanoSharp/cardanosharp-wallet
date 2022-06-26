using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardanoSharp.Wallet.Extensions.Models
{
	public static class UtxosExtensions
	{
		public static List<TransactionInput> GetTransactionInputs(this List<Utxo> utxos)
		{
			var transactionInputs = new List<TransactionInput>();

			foreach (var utxo in utxos)
			{
				transactionInputs.Add(new TransactionInput()
				{
					TransactionId = utxo.TxHash.HexToByteArray(),
					TransactionIndex = utxo.TxIndex
				});
			}

			return transactionInputs;
		}

		public static CBORObject GetCBOR(this Utxo utxo)
		{
			var inputArr = CBORObject.NewArray()
				.Add(utxo.TxHash.HexToByteArray())
				.Add(utxo.TxIndex);

			var outputArr = CBORObject.NewArray()
				.Add(utxo.OutputAddress.HexToByteArray());

			if (utxo.Balance.Assets != null && utxo.Balance.Assets.Count() > 0)
			{
				var assetArr = CBORObject.NewArray()
					.Add(utxo.Balance.Lovelaces);

				var multiAssetMap = CBORObject.NewMap();
				var groupedPolicies = utxo.Balance.Assets.GroupBy(x => x.PolicyId).ToDictionary(x => x.Key, x => x.ToList());

				foreach (var policy in groupedPolicies)
				{
					var policyMap = CBORObject.NewMap();
					foreach (var asset in policy.Value)
					{
						policyMap.Add(asset.Name, asset.Quantity);
					}
					multiAssetMap.Add(policy.Key, policyMap);
				}

				assetArr.Add(multiAssetMap);

				outputArr.Add(assetArr);
			}
			else
			{
				outputArr.Add(utxo.Balance.Lovelaces);
			}

			return CBORObject.NewArray()
				.Add(inputArr)
				.Add(outputArr);
		}

		public static Utxo GetUtxo(this CBORObject utxoCbor)
		{
			if (utxoCbor == null)
			{
				throw new ArgumentNullException(nameof(utxoCbor));
			}
			if (utxoCbor.Type != CBORType.Array)
			{
				throw new ArgumentException("transactionOutputCbor is not expected type CBORType.Map");
			}
			if (utxoCbor.Values.Count != 2)
			{
				throw new ArgumentException("transactionInputCbor unexpected number elements (expected 2)");
			}

			var tempInput = utxoCbor[0].GetTransactionInput();
			var tempOutput = utxoCbor[1].GetTransactionOutput();

			var utxo = new Utxo();
			utxo.TxHash = tempInput.TransactionId.ToStringHex();
			utxo.TxIndex = tempInput.TransactionIndex;
			utxo.OutputAddress = tempOutput.Address.ToStringHex();
			utxo.Balance.Lovelaces = tempOutput.Value.Coin;
			if (tempOutput.Value.MultiAsset != null && tempOutput.Value.MultiAsset.Count > 0)
			{
				utxo.Balance.Assets = tempOutput.Value.MultiAsset.SelectMany(
					x => x.Value.Token.Select(
						y => new Asset() { PolicyId = x.Key.ToStringHex(), Name = y.Key.ToStringAscii(), Quantity = y.Value }
						)
					).ToList();
			}
			return utxo;
		}

		public static byte[] Serialize(this Utxo utxo)
		{
			return utxo.GetCBOR().EncodeToBytes();
		}

		public static Utxo DeserializeUtxo(this byte[] bytes)
		{
			return CBORObject.DecodeFromBytes(bytes).GetUtxo();
		}
	}
}