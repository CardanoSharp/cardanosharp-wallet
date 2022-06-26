using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
	public class TransactionUnspentOutput
	{
		public TransactionInput Input { get; set; }

		public TransactionOutput Output { get; set; }
	}
}