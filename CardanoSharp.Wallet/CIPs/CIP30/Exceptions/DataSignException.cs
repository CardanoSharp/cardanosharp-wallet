using CardanoSharp.Wallet.CIPs.CIP30.Models.Errors;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP30.Exceptions
{
	public class DataSignException : ErrorCodeException
	{
		public DataSignException()
		{
		}

		public DataSignException(string message)
			: base(message)
		{
		}

		public DataSignException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public DataSignException(InfoCodeError error, string message, Exception inner)
			: base(error, message, inner)
		{
		}
	}
}