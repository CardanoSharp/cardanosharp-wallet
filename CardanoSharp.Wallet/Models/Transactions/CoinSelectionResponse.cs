using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public class CoinSelectionResponse
    {
        public List<TransactionInput> Inputs { get; set; }
        public List<TransactionOutput> ChangeOutputs { get; set; }
    }
}