namespace CardanoSharp.Wallet.CIPs.CIP30.Models.Errors
{
	public class InfoCodeError
	{
		public const string Schema = @"{ 'code': {'type':'int'}, 'info': {'type':'string'} }";

		public int code { get; set; }

		public string? info { get; set; }
	}
}