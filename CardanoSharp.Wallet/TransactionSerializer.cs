using CardanoSharp.Wallet.Models.Transactions;

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
