using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public class TransactionUnspentOutput
    {
        public List<TransactionInput> Inputs { get; set; }
        public List<TransactionOutput> Outputs { get; set; }
    }
}