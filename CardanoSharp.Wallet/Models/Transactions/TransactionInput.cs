namespace CardanoSharp.Wallet.Models.Transactions
{

    
//transaction_input = [transaction_id: $hash32
//                    , index : uint
//                    ]
    public partial class TransactionInput
    {
        public byte[] Id { get; set; }
        public uint TransactionInputIndex { get; set; }
    }
}