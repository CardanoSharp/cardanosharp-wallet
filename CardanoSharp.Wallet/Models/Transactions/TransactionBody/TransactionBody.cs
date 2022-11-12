using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{

    // transaction_body =
    // { 0 : set<transaction_input>    ; inputs
    // , 1 : [* transaction_output]
    // , 2 : coin                      ; fee
    // , ? 3 : uint                    ; time to live
    // , ? 4 : [* certificate]
    // , ? 5 : withdrawals
    // , ? 6 : update
    // , ? 7 : auxiliary_data_hash
    // , ? 8 : uint                    ; validity interval start
    // , ? 9 : mint
    // , ? 11 : script_data_hash
    // , ? 13 : set<transaction_input> ; collateral inputs
    // , ? 14 : required_signers
    // , ? 15 : network_id
    // , ? 16 : transaction_output     ; collateral return; New
    // , ? 17 : coin                   ; total collateral; New
    // , ? 18 : set<transaction_input> ; reference inputs; New
    // }

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
    }
}