namespace CardanoSharp.Wallet.Models.Transactions
{

    
//transaction_input = [transaction_id: $hash32
//                    , index : uint
//                    ]
    public partial class TransactionInput
    {
        public byte[] TransactionId { get; set; }
        public uint TransactionIndex { get; set; }
    }
}