using CardanoSharp.Wallet.Enums;

namespace CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts
{
    public class Redeemer
    {
        public RedeemerTag Tag { get; set; }
        public uint Index { get; set; }
        public IPlutusData PlutusData { get; set; }
        public ExUnits ExUnits { get; set; }
    }
}