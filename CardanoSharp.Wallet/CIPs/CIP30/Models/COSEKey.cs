namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
	public class COSEKey
	{
		public byte[] Key { get; set; } = null!;

		public byte[]? Kid { get; set; }
	}
}