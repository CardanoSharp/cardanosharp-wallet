namespace CardanoSharp.Wallet.Models.Transactions
{
    //vkeywitness = [ $vkey, $signature]
    public partial class VKeyWitness
    {
        public byte[] VKey { get; set; }
        public byte[] Signature { get; set; }
    }
}