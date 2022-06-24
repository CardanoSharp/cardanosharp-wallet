using System.Collections.Generic;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class UtxosExtensions
    {
        public static List<TransactionInput> GetTransactionInputs(this List<Utxo> utxos)
        {
            var transactionInputs = new List<TransactionInput>();

            foreach (var utxo in utxos)
            {
                transactionInputs.Add(new TransactionInput()
                {
                    TransactionId = utxo.TxHash.HexToByteArray(),
                    TransactionIndex = utxo.TxIndex
                });
            }

            return transactionInputs;
        }
    }
}