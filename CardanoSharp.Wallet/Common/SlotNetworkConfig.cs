namespace CardanoSharp.Wallet.Common
{
    public class SlotNetworkConfig
    {
        public long ZeroTime { get; set; }
        public long ZeroSlot { get; set; }
        public int SlotLength { get; set; }

        public SlotNetworkConfig() {}
        
        public SlotNetworkConfig(long ZeroTime, long ZeroSlot, int SlotLength)
        {
            this.ZeroTime = ZeroTime;
            this.ZeroSlot = ZeroSlot;
            this.SlotLength = SlotLength;
        }
    }
}
