namespace CardanoSharp.Wallet.Models.Keys
{
    public class PublicKey
    {
        
        public byte[] Key { get; set; }
        public byte[] Chaincode { get; set; }

        public PublicKey(byte[] key, byte[] chaincode)
        {
            Key = key;
            Chaincode = chaincode;
        }
    }
}
