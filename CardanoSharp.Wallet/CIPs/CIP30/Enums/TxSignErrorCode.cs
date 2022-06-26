namespace CardanoSharp.Wallet.CIPs.CIP30.Enums
{
	public enum TxSignErrorCode
	{
		/// <summary>
		/// User has accepted the transaction sign, but the wallet was unable to sign the transaction
		/// (e.g. not having some of the private keys)
		/// </summary>
		ProofGeneration = 1,

		/// <summary>
		/// User declined to sign the transaction
		/// </summary>
		UserDeclined = 2,
	}
}