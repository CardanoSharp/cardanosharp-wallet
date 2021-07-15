using CardanoSharp.Wallet.Models.Transactions.Scripts;

namespace CardanoSharp.Wallet.Models.Transactions
{
    //  native_script =
    //[script_pubkey
    //// script_all
    //// script_any
    //// script_n_of_k
    //// invalid_before
    //   ; Timelock validity intervals are half-open intervals [a, b).
    //   ; This field specifies the left(included) endpoint a.
    //// invalid_hereafter
    //   ; Timelock validity intervals are half-open intervals [a, b).
    //   ; This field specifies the right(excluded) endpoint b.
    //]
    public partial class NativeScript
    {
        public ScriptPubKey ScriptPubKey { get; set; }
        public ScriptAll ScriptAll { get; set; }
        public ScriptAny ScriptAny { get; set; }
        public ScriptNofK ScriptNofK { get; set; }
        // invalid_before = (4, uint)
        public uint InvalidBefore { get; set; }
        // invalid_hereafter = (5, uint)
        public uint InvalidHereafter { get; set; }
    }
}