using CardanoSharp.Wallet.Models.Transactions.Scripts;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.Scripts;

namespace CardanoSharp.Wallet.Models.Transactions
{
    //  native_script =
    //     [script_pubkey
    //     // script_all
    //     // script_any
    //     // script_n_of_k
    //     // invalid_before
    //        ; Timelock validity intervals are half-open intervals [a, b).
    //        ; This field specifies the left(included) endpoint a.
    //     // invalid_hereafter
    //        ; Timelock validity intervals are half-open intervals [a, b).
    //        ; This field specifies the right(excluded) endpoint b.
    //     ]
    public partial class NativeScript
    {
        public ScriptPubKey ScriptPubKey { get; set; }
        public ScriptAll ScriptAll { get; set; }
        public ScriptAny ScriptAny { get; set; }
        public ScriptNofK ScriptNofK { get; set; }
        public ScriptInvalidAfter InvalidAfter { get; set; }
        public ScriptInvalidBefore InvalidBefore { get; set; }

    }
}