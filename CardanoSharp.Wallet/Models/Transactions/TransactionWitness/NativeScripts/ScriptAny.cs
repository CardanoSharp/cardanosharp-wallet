using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.NativeScripts
{
    // script_any = (2, [ * native_script ])
    public class ScriptAny
    {
        public ScriptAny()
        {
            NativeScripts = new List<NativeScript>();
        }

        public IList<NativeScript> NativeScripts { get; set; }
    }
}
