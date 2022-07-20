namespace CardanoSharp.Wallet.CIPs.CIP30.Enums
{
	public enum APIErrorCode
	{
		/// <summary>
		/// Inputs do not conform to this spec or are otherwise invalid.
		/// </summary>
		InvalidRequest = -1,

		/// <summary>
		/// An error occurred during execution of this API call.
		/// </summary>
		InternalError = -2,

		/// <summary>
		/// The request was refused due to lack of access - e.g. wallet disconnects.
		/// </summary>
		Refused = -3,

		/// <summary>
		/// The account has changed. The dApp should call wallet.enable() to reestablish connection to the new account.
		/// The wallet should not ask for confirmation as the user was the one who initiated the account change in the first place.
		/// </summary>
		AccountChange = -4,
	}
}