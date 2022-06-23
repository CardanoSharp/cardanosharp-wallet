namespace CardanoSharp.Wallet.Models.Keys
{
    public class PrivateKey
    {
        public byte[] Key { get; }
        public byte[] Chaincode { get; }

        public PrivateKey(byte[] key, byte[] chaincode)
        {
            Key = key;
            Chaincode = chaincode;
        }
    }
}
