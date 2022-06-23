using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionInputBuilder : IABuilder<TransactionInput>
    {
        ITransactionInputBuilder WithTransactionId(byte[] transactionId);

        ITransactionInputBuilder WithTransactionIndex(uint transactionIndex);
    }

    public class TransactionInputBuilder: ABuilder<TransactionInput>, ITransactionInputBuilder
    {
        public TransactionInputBuilder()
        {
            _model = new TransactionInput();
        }

        private TransactionInputBuilder(TransactionInput model)
        {
            _model = model;
        }

        public static ITransactionInputBuilder GetBuilder(TransactionInput model)
        {
            if (model == null)
            {
                return new TransactionInputBuilder();
            }
            return new TransactionInputBuilder(model);
        }

        public ITransactionInputBuilder WithTransactionId(byte[] transactionId)
        {
            _model.TransactionId = transactionId;
            return this;
        }

        public ITransactionInputBuilder WithTransactionIndex(uint transactionIndex)
        {
            _model.TransactionIndex = transactionIndex;
            return this;
        }
    }
}
