using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness
{
    //MultiSig: https://docs.cardano.org/projects/cardano-node/en/latest/reference/simple-scripts.html#type-all

    // transaction_witness_set =
    // { ? 0: [* vkeywitness ]
    //         , ? 1: [* native_script ]
    //         , ? 2: [* bootstrap_witness ]
    //         , ? 3: [* plutus_v1_script ]
    //         , ? 4: [* plutus_data ]
    //         , ? 5: [* redeemer ]
    //         , ? 6: [* plutus_v2_script ] ; New
    // }

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
        public ICollection<PlutusScriptV1> PlutusScriptV1 { get; set; }
        public ICollection<IPlutusData> PlutusData { get; set; }
        public ICollection<Redeemer> Redeemer { get; set; }
        public ICollection<PlutusScriptV2> PlutusScriptV2 { get; set; }
        
    }
}
