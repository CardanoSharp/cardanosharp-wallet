using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions
{

    //  transaction_witness_set =
    //{ ? 0: [* vkeywitness ]
    //, ? 1: [* native_script ]
    //, ? 2: [* bootstrap_witness ]
    //; In the future, new kinds of witnesses can be added like this:
    //; , ? 4: [* foo_script]
    //; , ? 5: [* plutus_script]
    //}
    public partial class TransactionWitnessSet
    {
        public TransactionWitnessSet()
        {
            VKeyWitnesses = new HashSet<VKeyWitness>();
            NativeScripts = new HashSet<NativeScript>();
        }
        public ICollection<VKeyWitness> VKeyWitnesses { get; set; }
        public ICollection<NativeScript> NativeScripts { get; set; }


    }
}
