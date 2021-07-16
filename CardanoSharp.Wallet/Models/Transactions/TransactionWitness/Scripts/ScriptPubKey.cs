using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Transactions.Scripts
{
    public class ScriptPubKey
    {
        // addr_keyhash = $hash28
        public byte[] KeyHash { get; set; }
    }
}
