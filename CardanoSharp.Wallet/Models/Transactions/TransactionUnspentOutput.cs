using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public class TransactionUnspentOutput
    {
        public TransactionInput Input { get; set; }
        public TransactionOutput Output { get; set; }
    }
}
