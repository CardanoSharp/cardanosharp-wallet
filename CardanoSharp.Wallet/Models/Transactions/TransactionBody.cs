using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{

// transaction_body =
//  { 0 : set<transaction_input>
//  , 1 : [* transaction_output]
//  , 2 : coin ; fee
//  , ? 3 : uint ; ttl
//  , ? 4 : [* certificate]
//  , ? 5 : withdrawals
//  , ? 6 : update
//  , ? 7 : metadata_hash
//  , ? 8 : uint ; validity interval start
//  , ? 9 : mint
//}

public partial class TransactionBody
    {
        public TransactionBody()
        {
            TransactionInputs = new HashSet<TransactionInput>();
            TransactionOutputs = new HashSet<TransactionOutput>(); 

        }

        public virtual ICollection<TransactionInput> TransactionInputs { get; set; }
        public virtual ICollection<TransactionOutput> TransactionOutputs { get; set; }
        public uint Coin { get; set; }
    }
}