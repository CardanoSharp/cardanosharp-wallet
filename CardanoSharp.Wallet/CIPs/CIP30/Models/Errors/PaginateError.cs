namespace CardanoSharp.Wallet.CIPs.CIP30.Models.Errors
{
	public class PaginateError
	{
		public const string Schema = @"{ 'maxSize': {'type':'int'} }";

		public int maxSize { get; set; }
	}
}