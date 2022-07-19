using System.Threading.Tasks;

namespace CardanoSharp.Blazor.Components.Interfaces
{
	/// <summary>
	/// CIP30 Compliant initial interface as per https://cips.cardano.org/cips/cip30/
	/// Initial API
	/// In order to initiate communication from webpages to a user's Cardano wallet, the wallet must provide the
	/// following javascript API to the webpage. A shared, namespaced cardano object must be injected into the page
	/// if it did not exist already. Each wallet implementing this standard must then create a field in this object
	/// with a name unique to each wallet containing a wallet object with the following methods. The API is split into
	/// two stages to maintain the user's privacy, as the user will have to consent to cardano.{walletName}.enable()
	/// in order for the dApp to read any information pertaining to the user's wallet with {walletName} corresponding
	/// to the wallet's namespaced name of its choice.
	/// </summary>
	public interface IWebWalletInitialApi
	{
		/// <summary>
		/// cardano.{walletName}.enable(): Promise\
		/// Errors: APIError
		/// This is the entrypoint to start communication with the user's wallet. The wallet should request the
		/// user's permission to connect the web page to the user's wallet, and if permission has been granted,
		/// the full API will be returned to the dApp to use. The wallet can choose to maintain a whitelist to not
		/// necessarily ask the user's permission every time access is requested, but this behavior is up to the wallet
		/// and should be transparent to web pages using this API. If a wallet is already connected this function should
		/// not request access a second time, and instead just return the API object.
		/// </summary>
		/// <returns></returns>
		Task<object> Enable();

		/// <summary>
		/// cardano.{walletName}.isEnabled(): Promise\
		/// Errors: APIError
		/// Returns true if the dApp is already connected to the user's wallet, or if requesting access would return
		/// true without user confirmation (e.g. the dApp is whitelisted), and false otherwise. If this function returns
		/// true, then any subsequent calls to wallet.enable() during the current session should succeed and return the API
		/// object.
		/// </summary>
		/// <returns></returns>
		Task<bool> IsEnabled();

		/// <summary>
		/// cardano.{walletName}.apiVersion: String
		/// The version number of the API that the wallet supports.
		/// method instead attribute and return type is ValueTask<string> instead of string as we still have to invoke
		/// js interop which is async
		/// </summary>
		Task<string> ApiVersion();

		/// <summary>
		/// cardano.{walletName}.name: String
		/// A name for the wallet which can be used inside of the dApp for the purpose of asking the user which
		/// wallet they would like to connect with.
		/// method instead attribute and return type is ValueTask<string> instead of string as we still have to invoke
		/// js interop which is async
		/// </summary>
		Task<string> Name();

		/// <summary>
		/// cardano.{walletName}.icon: String
		/// A URI image (e.g. data URI base64 or other) for img src for the wallet which can be used inside of the
		/// dApp for the purpose of asking the user which wallet they would like to connect with.
		/// method instead attribute and return type is ValueTask<string> instead of string as we still have to invoke
		/// js interop which is async
		/// </summary>
		Task<string> Icon();
	}
}