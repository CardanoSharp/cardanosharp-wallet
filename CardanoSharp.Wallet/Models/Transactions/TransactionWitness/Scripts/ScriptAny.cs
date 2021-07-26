using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions.Scripts
{
    // script_any = (2, [ * native_script ])
    public class ScriptAny
    {
        public List<NativeScript> NativeScripts { get; set; }
    }
}
