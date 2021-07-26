using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions.Scripts
{
    // script_n_of_k = (3, n: uint, [ * native_script ])
    public class ScriptNofK
    {
        public uint N { get; set; }
        public List<NativeScript> NativeScripts { get; set; }
    }
}
