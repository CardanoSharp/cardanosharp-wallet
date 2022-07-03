namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
	/// <summary>
	/// https://cips.cardano.org/cips/cip30/#paginate
	/// Paginate
	///    type Paginate = {|
	///      page: number,
	///      limit: number,
	///    |};
	/// Used to specify optional pagination for some API calls.Limits results to {limit} each page,
	/// and uses a 0-indexing {page} to refer to which of those pages of {limit} items each.dApps should be aware
	/// that if a wallet is modified between paginated calls that this will change the pagination, e.g.some results
	/// skipped or showing up multiple times but otherwise the wallet must respect the pagination order.
	/// </summary>
	public class Paginate
	{
		public int Page { get; set; }

		public int Limit { get; set; }

		public Paginate()
		{ }

		public Paginate(int page, int limit)
		{
			Page = page;
			Limit = limit;
		}
	}
}