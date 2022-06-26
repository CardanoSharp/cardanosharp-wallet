using CardanoSharp.Wallet.CIPs.CIP30.Models.Errors;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP30.Exceptions
{
	public class ErrorCodeException : WebWalletException
	{
		public int Code { get; set; }

		public string? Info { get; set; }

		public ErrorCodeException()
		{
		}

		public ErrorCodeException(string message)
			: base(message)
		{
		}

		public ErrorCodeException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public ErrorCodeException(InfoCodeError error, string message, Exception inner)
			: base(message, inner)
		{
			Code = error.code;
			Info = error.info;
		}
	}
}