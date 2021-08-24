using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions.Scripts
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
