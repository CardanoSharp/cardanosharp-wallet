using System;

namespace CardanoSharp.Wallet.CIPs.CIP30.Exceptions
{
	public class WebWalletException : Exception
	{
		public WebWalletException()
		{
		}

		public WebWalletException(string message)
			: base(message)
		{
		}

		public WebWalletException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}