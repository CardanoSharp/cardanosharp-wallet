namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
	public class CoseSign1
	{
		public byte[] Signature { get; set; } = null!;

		public byte[] Headers { get; set; } = null!;

		public byte[] Payload { get; set; } = null!;

		public bool Hashed { get; set; } = false;

		public byte[] GetPayload()
		{
			return Payload;
		}
	}
}