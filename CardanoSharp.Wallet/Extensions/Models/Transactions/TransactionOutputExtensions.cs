
using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
	public static partial class TransactionOutputExtensions
	{
		private static ulong adaOnlyMinUTxO = 1000000;
		private static string dummyAddress = "addr_test1qpu5vlrf4xkxv2qpwngf6cjhtw542ayty80v8dyr49rf5ewvxwdrt70qlcpeeagscasafhffqsxy36t90ldv06wqrk2qum8x5w";

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
			ulong coinsPerUtxOByte = 4310 // coinsPerUtxoByte in protocol params
			)
		{
			if (output.Value.MultiAsset == null || output.Value.MultiAsset.Count <= 0)
				return adaOnlyMinUTxO;

			// Set a dummy coin value if coin is 0
			bool setDummyCoin = false;
			if (output.Value.Coin == 0)
			{
				setDummyCoin = true;
				output.Value.Coin = adaOnlyMinUTxO;
			}

			// Set a dummy address if this function is called with Address == null
			if (output.Address == null)
				output.Address = new Address(dummyAddress).GetBytes();

			byte[] serializedOutput = output.Serialize();
			ulong outputLength = (ulong)serializedOutput.Length;
			ulong minUTxO = coinsPerUtxOByte * (160 + outputLength);
			if (minUTxO < adaOnlyMinUTxO)
				minUTxO = adaOnlyMinUTxO;

			if (setDummyCoin) {
				output.Value.Coin = 0;
			}

			return minUTxO;
		}

		//maxOutputBytesSize is a Protocol Parameter and may change in the future
		public static bool IsValid(this TransactionOutput output, ulong maxOutputBytesSize = 5000) {
			byte[] serializedOutput = output.Serialize();
			ulong outputLength = (ulong)serializedOutput.Length;
			if (outputLength > maxOutputBytesSize)
				return false;
			return true;
		}
	}
}
