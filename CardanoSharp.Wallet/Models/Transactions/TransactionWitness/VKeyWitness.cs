using CardanoSharp.Wallet.Models.Keys;

namespace CardanoSharp.Wallet.Models.Transactions
{
    //vkeywitness = [ $vkey, $signature]
    public partial class VKeyWitness
    {
        public PublicKey VKey { get; set; }
        public PrivateKey SKey { get; set; }
        public byte[] Signature { get; set; }
        public bool IsMock { get; set; }
    }
}