namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    public partial class ScriptReference
    {
        public NativeScript? NativeScript { get; set; }
        public PlutusV1Script? PlutusV1Script { get; set; }
        public PlutusV2Script? PlutusV2Script { get; set; }
    }
}