using CardanoSharp.Wallet.CIPs.CIP30.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP30.Extensions.Models
{
	public static class TransactionUnspentOutputExtensions
	{
		public static CBORObject GetCBOR(this TransactionUnspentOutput transactionUnspentOutput)
		{
			return CBORObject.NewArray()
				.Add(transactionUnspentOutput.Input.GetCBOR())
				.Add(transactionUnspentOutput.Output.GetCBOR());
		}

		public static TransactionUnspentOutput GetTransactionUnspentOutput(this CBORObject transactionUnspentOutputCbor)
		{
			//validation
			if (transactionUnspentOutputCbor == null)
			{
				throw new ArgumentNullException(nameof(transactionUnspentOutputCbor));
			}
			if (transactionUnspentOutputCbor.Type != CBORType.Array)
			{
				throw new ArgumentException("transactionUnspentOutputCbor is not expected type CBORType.Array");
			}
			if (transactionUnspentOutputCbor.Values.Count != 2)
			{
				throw new ArgumentException("transactionInputCbor unexpected number elements (expected 2)");
			}

			//get data
			var unspentOutput = new TransactionUnspentOutput();
			unspentOutput.Input = transactionUnspentOutputCbor[0].GetTransactionInput();
			unspentOutput.Output = transactionUnspentOutputCbor[1].GetTransactionOutput();

			//return
			return unspentOutput;
		}

		public static byte[] Serialize(this TransactionUnspentOutput transactionUnspentOutput)
		{
			return transactionUnspentOutput.GetCBOR().EncodeToBytes();
		}

		public static TransactionUnspentOutput DeserializeTransactionUnspentOutput(this byte[] bytes)
		{
			return CBORObject.DecodeFromBytes(bytes).GetTransactionUnspentOutput();
		}
	}
}