using System.Collections.Generic;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2.Models
{
    public class CoinSelection
    {
        public CoinSelection()
        {
            Inputs = new List<TransactionInput>();
            ChangeOutputs = new List<TransactionOutput>();
            SelectedUtxos = new List<Utxo>();
        }

        public List<Utxo> SelectedUtxos { get; set; }
        public List<TransactionInput> Inputs { get; set; }
        public List<TransactionOutput> ChangeOutputs { get; set; }
    }
}