using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
	/// <summary>
	/// ref: https://cips.cardano.org/cips/cip30/
	/// TransactionUnspentOutput
	/// If we have CBOR specified by the following CDDL referencing the Shelley-MA CDDL:
	/// transaction_unspent_output = [
	///  input: transaction_input,
	///  output: transaction_output,
	/// ]
	/// then we define
	/// type TransactionUnspentOutput = cbor<transaction_unspent_output>
	/// This allows us to use the output for constructing new transactions using it as an
	/// output as the transaction_output in the Shelley Multi-asset CDDL does not contain
	/// enough information on its own to spend it.
	/// </summary>
	public class TransactionUnspentOutput
	{
		public TransactionInput Input { get; set; }

		public TransactionOutput Output { get; set; }
	}
}