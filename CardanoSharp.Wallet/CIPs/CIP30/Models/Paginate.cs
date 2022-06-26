namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
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