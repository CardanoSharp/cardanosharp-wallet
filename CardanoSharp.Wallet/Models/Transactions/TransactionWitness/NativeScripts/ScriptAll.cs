using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.NativeScripts
{
    // script_all = (1, [ * native_script ])
    public class ScriptAll
    {
        public ScriptAll()
        {
            NativeScripts = new HashSet<NativeScript>();
        }

        public ICollection<NativeScript> NativeScripts { get; set; }
    }
}
