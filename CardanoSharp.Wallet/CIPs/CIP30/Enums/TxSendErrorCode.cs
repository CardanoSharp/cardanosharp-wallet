namespace CardanoSharp.Wallet.CIPs.CIP30.Enums
{
	public enum TxSendErrorCode
	{
		/// <summary>
		/// Wallet refuses to send the tx (could be rate limiting)
		/// </summary>
		Refused = 1,

		/// <summary>
		/// Wallet could not send the tx
		/// </summary>
		Failure = 2,
	}
}