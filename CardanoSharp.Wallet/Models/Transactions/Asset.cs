namespace CardanoSharp.Wallet.Models.Transactions
{
    public class Asset
    {
        public byte[] PolicyId { get; set; }
        public byte[] Name { get; set; }
        public ulong Quantity { get; set; }
    }
}