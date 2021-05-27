using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public class Transaction
    {
        public TransactionBody TransactionBody { get; set; }
        public TransactionWitnessSet TransactionWitnessSet { get; set; }
        public AuxiliaryData AuxiliaryData { get; set; }
    }
}
