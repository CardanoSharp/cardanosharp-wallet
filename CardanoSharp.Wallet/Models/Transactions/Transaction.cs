namespace CardanoSharp.Wallet.Models.Transactions
{
    /// <summary>
    /// Parent class to create transactions. If a child class does not have
    /// a comment description, it means that it is a Rust CBOR libary specific
    /// type, and type Rust type will match the C# type exactly and it would be
    /// repetitive to put the comment.
    /// Rust libary used for reference -> https://github.com/Emurgo/cardano-serialization-lib/blob/master/rust/src/lib.rs
    /// </summary>
    public class Transaction
    {
        public TransactionBody TransactionBody { get; set; }
        public TransactionWitnessSet TransactionWitnessSet { get; set; }
        public bool IsValid { get; set; } = true;
        public AuxiliaryData AuxiliaryData { get; set; }
    }
}
