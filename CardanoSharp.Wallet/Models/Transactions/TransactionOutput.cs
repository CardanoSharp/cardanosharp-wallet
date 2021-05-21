namespace CardanoSharp.Wallet.Models.Transactions
{

    //transaction_output = [address, amount : value]
    public partial class TransactionOutput
    {
        public byte[] Id { get; set; }
    }
}