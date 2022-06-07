using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions.Scripts
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
