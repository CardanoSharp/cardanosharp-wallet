using CardanoSharp.Wallet.CIPs.CIP30.Enums;

namespace CardanoSharp.Wallet.CIPs.CIP30.Models.Errors
{
	public class TxSignError
	{
		public TxSignErrorCode Code { get; set; }

		public string Info { get; set; }
	}
}