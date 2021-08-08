using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Models.Transactions;
using Chaos.NaCl;
using PeterO.Cbor2;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Utilities;
using System.Linq;
using CardanoSharp.Wallet.Extensions.Models;

namespace CardanoSharp.Wallet
{

    public interface ITransactionSerializer
    {
        TransactionBody DeserializeBody(byte[] transactionBody);
        Transaction DeserializeTransaction(byte[] transaction);
    }

    public class TransactionSerializer : ITransactionSerializer
    {
        public TransactionBody DeserializeBody(byte[] transactionBody)
        {
            throw new System.NotImplementedException();
        }

        public Transaction DeserializeTransaction(byte[] transaction)
        {
            throw new System.NotImplementedException();
        }
    }
}
