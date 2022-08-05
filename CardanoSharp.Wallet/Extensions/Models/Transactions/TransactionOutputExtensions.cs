using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
	public static partial class TransactionOutputExtensions
	{
		public static CBORObject GetCBOR(this TransactionOutput transactionOutput)
		{
			//start the cbor transaction output object with the address we are sending
			var cborTransactionOutput = CBORObject.NewArray()
				.Add(transactionOutput.Address)
				.Add(transactionOutput.Value.GetCBOR());

			return cborTransactionOutput;
		}

		public static TransactionOutput GetTransactionOutput(this CBORObject transactionOutputCbor)
		{
			//validation
			if (transactionOutputCbor == null)
			{
				throw new ArgumentNullException(nameof(transactionOutputCbor));
			}
			if (transactionOutputCbor.Type != CBORType.Array)
			{
				throw new ArgumentException("transactionOutputCbor is not expected type CBORType.Map");
			}
			if (transactionOutputCbor.Count != 2)
			{
				throw new ArgumentException("transactionOutputCbor unexpected number elements (expected 2)");
			}
			if (transactionOutputCbor[0].Type != CBORType.ByteString)
			{
				throw new ArgumentException("transactionOutputCbor first element unexpected type (expected ByteString)");
			}
			if (transactionOutputCbor[1].Type != CBORType.Integer && transactionOutputCbor[1].Type != CBORType.Array)
			{
				throw new ArgumentException("transactionInputCbor second element unexpected type (expected Integer or Array)");
			}

			//get data
			var transactionOutput = new TransactionOutput();
			transactionOutput.Address = ((string)transactionOutputCbor[0].DecodeValueByCborType()).HexToByteArray();
			if (transactionOutputCbor[1].Type == CBORType.Integer)
			{
				//coin
				transactionOutput.Value = new TransactionOutputValue()
				{
					Coin = transactionOutputCbor[1].DecodeValueToUInt64()
				};
			}
			else
			{
				//multi asset
				transactionOutput.Value = new TransactionOutputValue();
				transactionOutput.Value.MultiAsset = new Dictionary<byte[], NativeAsset>();

				var coinCbor = transactionOutputCbor[1][0];
				transactionOutput.Value.Coin = coinCbor.DecodeValueToUInt64();

				var multiAssetCbor = transactionOutputCbor[1][1];
				foreach (var policyKeyCbor in multiAssetCbor.Keys)
				{
					var nativeAsset = new NativeAsset();

					var assetMapCbor = multiAssetCbor[policyKeyCbor];
					var policyKeyBytes = ((string)policyKeyCbor.DecodeValueByCborType()).HexToByteArray();

					foreach (var assetKeyCbor in assetMapCbor.Keys)
					{
						var assetToken = assetMapCbor[assetKeyCbor].DecodeValueToInt64();
						var assetKeyBytes = ((string)assetKeyCbor.DecodeValueByCborType()).HexToByteArray();

						nativeAsset.Token.Add(assetKeyBytes, assetToken);
					}

					transactionOutput.Value.MultiAsset.Add(policyKeyBytes, nativeAsset);
				}
			}

			//return
			return transactionOutput;
		}

		public static byte[] Serialize(this TransactionOutput transactionOutput)
		{
			return transactionOutput.GetCBOR().EncodeToBytes();
		}

		public static TransactionOutput DeserializeTransactionOutput(this byte[] bytes)
		{
			return CBORObject.DecodeFromBytes(bytes).GetTransactionOutput();
		}

		public static ulong CalculateMinUtxoLovelace(
			this TransactionOutput output,
			int lovelacePerUtxoWord = 34482, // utxoCostPerWord in protocol params (could change in the future)
			int policyIdSizeBytes = 28, // 224 bit policyID (won't change in forseeable future)
			bool hasDataHash = false) // for UTxOs with a smart contract datum
		{
			const int fixedUtxoEntryWithoutValueSizeWords = 27; // The static parts of a UTxO: 6 + 7 + 14 words
			const int coinSizeWords = 2; // since updated from 0 in docs.cardano.org/native-tokens/minimum-ada-value-requirement
			const int adaOnlyUtxoSizeWords = fixedUtxoEntryWithoutValueSizeWords + coinSizeWords;

			var nativeAssets = (output.Value.MultiAsset != null && output.Value.MultiAsset.Count > 0);

			if (!nativeAssets)
				return (ulong)lovelacePerUtxoWord * adaOnlyUtxoSizeWords; // 999978 lovelaces or 0.999978 ADA

			return output.Value.MultiAsset.CalculateMinUtxoLovelace(lovelacePerUtxoWord, policyIdSizeBytes, hasDataHash);
		}
	}
}