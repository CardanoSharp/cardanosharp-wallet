namespace CardanoSharp.Wallet.Models.Transactions
{
 //   bootstrap_witness =
	//[public_key: $vkey
	//, signature : $signature
	//, chain_code : bytes.size 32
	//, attributes : bytes
	//]
	public class BootStrapWitness
    {
        public byte[] Signature { get; set; }
        public byte[] ChainNode { get; set; }
        public byte[] Attributes { get; set; }
    }
}