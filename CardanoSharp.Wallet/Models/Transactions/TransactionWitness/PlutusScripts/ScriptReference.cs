namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    public partial class ScriptReference
    {
        public NativeScript? NativeScript { get; set; }
        public byte[]? PlutusV1Script { get; set; }
        public byte[]? PlutusV2Script { get; set; }
    }
}