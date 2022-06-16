using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    //MultiSig: https://docs.cardano.org/projects/cardano-node/en/latest/reference/simple-scripts.html#type-all

    //  transaction_witness_set =
    //{ ? 0: [* vkeywitness ]
    //, ? 1: [* native_script ]
    //, ? 2: [* bootstrap_witness ]
    //; In the future, new kinds of witnesses can be added like this:
    //; , ? 4: [* foo_script]
    //; , ? 5: [* plutus_script]
    //}

    //This can be updated for future witness changes based upon CDDL changes
    public partial class TransactionWitnessSet
    {
        public TransactionWitnessSet()
        {
            VKeyWitnesses = new HashSet<VKeyWitness>();
            NativeScripts = new HashSet<NativeScript>();
        }
        public ICollection<VKeyWitness> VKeyWitnesses { get; set; }
        public ICollection<NativeScript> NativeScripts { get; set; }
        public ICollection<BootStrapWitness> BootStrapWitnesses { get; set; }


    }
}
