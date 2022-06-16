using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions.Scripts
{
    // script_n_of_k = (3, n: uint, [ * native_script ])
    public class ScriptNofK
    {
        public ScriptNofK()
        {
            NativeScripts = new List<NativeScript>();
        }

        public uint N { get; set; }
        public IList<NativeScript> NativeScripts { get; set; }
    }
}
