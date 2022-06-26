using CardanoSharp.Wallet.CIPs.CIP30.Models.Errors;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP30.Exceptions
{
	public class TxSendException : ErrorCodeException
	{
		public TxSendException()
		{
		}

		public TxSendException(string message)
			: base(message)
		{
		}

		public TxSendException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public TxSendException(InfoCodeError error, string message, Exception inner)
			: base(error, message, inner)
		{
		}
	}
}