using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    public partial class TransactionBody
    {
        public TransactionBody()
        {
            TransactionInputs = new List<TransactionInput>();
            TransactionOutputs = new List<TransactionOutput>();
            Mint = new Dictionary<byte[], NativeAsset>();
        }

        public virtual IList<TransactionInput> TransactionInputs { get; set; }
        public virtual IList<TransactionOutput> TransactionOutputs { get; set; }
        public ulong Fee { get; set; }
        public uint? Ttl { get; set; }
        public Certificate Certificate { get; set; }
        public Dictionary<byte[], uint> Withdrawls { get; set; }
        public Update Update { get; set; }
        public string MetadataHash { get; set; }
        public uint? TransactionStartInterval { get; set; }
        public Dictionary<byte[], NativeAsset> Mint { get; set; }
        public byte[] ScriptDataHash { get; set; }
        public virtual IList<TransactionInput> Collateral { get; set; }
        public virtual IList<byte[]> RequiredSigners { get; set; }
        public uint? NetworkId { get; set; }
        public TransactionOutput CollateralReturn { get; set; }
        public ulong? TotalCollateral { get; set; }
        public IList<TransactionInput> ReferenceInputs { get; set; } 
    }
}