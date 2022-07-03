namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
	/// <summary>
	/// https://cips.cardano.org/cips/cip30/#datasignature
	/// DataSignature
	/// type DataSignature = {|
	///   signature:cbor\<COSE_Sign1>,
	///   key: cbor\<COSE_Key>,
	/// |};
	/// </summary>
	public class DataSignature
	{
		/// <summary>
		/// cbor\<COSE_Sign1>
		/// </summary>
		public string? Signature { get; set; }

		/// <summary>
		/// cbor\<COSE_Key>
		/// </summary>
		public string? Key { get; set; }
	}
}