namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
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