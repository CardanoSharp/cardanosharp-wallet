namespace CardanoSharp.Wallet.Models.Keys
{
    public class Mnemonic
    {
        public string Words { get; }
        public byte[] Entropy { get; }

        public Mnemonic(string words, byte[] entropy)
        {
            Words = words;
            Entropy = entropy;
        }
    }
}
