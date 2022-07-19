namespace CardanoSharp.Wallet.CIPs.CIP30.Enums
{
	public enum DataSignErrorCode
	{
		/// <summary>
		/// Wallet could not sign the data (e.g. does not have the secret key associated with the address)
		/// </summary>
		ProofGeneration = 1,

		/// <summary>
		/// Address was not a P2PK address and thus had no SK associated with it.
		/// </summary>
		AddressNotPK = 2,

		/// <summary>
		/// User declined to sign the data
		/// </summary>
		UserDeclined = 3,
	}
}