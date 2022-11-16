namespace CardanoSharp.Wallet.CIPs.CIP30.Models
{
	public class COSESign1
	{
		public byte[] Signature { get; set; } = null!;

		public byte[] Headers { get; set; } = null!;

		public byte[] Payload { get; set; } = null!;

		public bool Hashed { get; set; } = false;

		public byte[]? PayloadHash { get; set; }

		public byte[] GetPayload()
		{
			if (Hashed && PayloadHash != null)
				return PayloadHash;
			return Payload;
		}
	}
}