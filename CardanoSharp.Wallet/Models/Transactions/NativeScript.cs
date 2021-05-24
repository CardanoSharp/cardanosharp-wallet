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
    }
}