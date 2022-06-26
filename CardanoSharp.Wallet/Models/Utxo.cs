namespace CardanoSharp.Wallet.Models
{
	public class Utxo
	{
		public string? TxHash { get; set; }

		public uint TxIndex { get; set; }

		public string? OutputAddress { get; set; }

		public Balance Balance { get; set; }
	}
}