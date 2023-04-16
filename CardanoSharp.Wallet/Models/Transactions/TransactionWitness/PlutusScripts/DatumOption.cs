namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    public partial class DatumOption
    {
        public byte[]? Hash { get; set; }
        public IPlutusData? Data { get; set; }
    }
}