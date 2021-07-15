using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions.Scripts
{
    // script_all = (1, [ * native_script ])
    public class ScriptAll
    {
        public List<NativeScript> NativeScripts { get; set; }
    }
}
