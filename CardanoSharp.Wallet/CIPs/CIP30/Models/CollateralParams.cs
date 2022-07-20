namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
	/// <summary>
	/// https://cips.cardano.org/cips/cip30/#apigetcollateralparamsamountcborcoinpromisetransactionunspentoutputnull
	/// params: { amount: cbor\ }
	/// The function takes a required object with parameters. With a single required parameter for now: amount.
	/// (NOTE: some wallets may be ignoring the amount parameter, in which case it might be possible to call the
	/// function without it, but this behavior is not recommended!).
	///
	/// The amount parameter is required, specified as a string (BigNumber) or a number, and the maximum allowed
	/// value must be agreed to be something like 5 ADA.
	/// </summary>
	public class CollateralParams
	{
		/// <summary>
		/// amount: cbor\
		/// A hex-encoded string representing CBOR
		/// </summary>
		public string Amount { get; set; }
	}
}