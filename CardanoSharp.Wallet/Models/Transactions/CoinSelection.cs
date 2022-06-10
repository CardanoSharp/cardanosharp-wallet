using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
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