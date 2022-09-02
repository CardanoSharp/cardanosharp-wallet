namespace CardanoSharp.Wallet.Common
{
    public class NetworkInfo
    {
        public int NetworkId { get; set; }
        public int NetworkMagic { get; set; }

        public NetworkInfo(int networkId, int networkMagic)
        {
            NetworkId = networkId;
            NetworkMagic = networkMagic;
        }
    }
}
