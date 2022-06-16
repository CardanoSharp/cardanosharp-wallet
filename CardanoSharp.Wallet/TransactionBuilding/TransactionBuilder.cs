using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionBuilder: IABuilder<Transaction>
    {
        ITransactionBuilder SetBody(ITransactionBodyBuilder bodyBuilder);
        ITransactionBuilder SetWitnesses(ITransactionWitnessSetBuilder witnessBuilder);
        ITransactionBuilder SetAuxData(IAuxiliaryDataBuilder auxDataBuilder);
    }

    public partial class TransactionBuilder : ABuilder<Transaction>, ITransactionBuilder
    {
        private TransactionBuilder()
        {
            _model = new Transaction();
        }

        private TransactionBuilder(Transaction model)
        {
            _model = model;
        }

        public static ITransactionBuilder GetBuilder(Transaction model)
        {
            if (model == null)
            {
                return new TransactionBuilder();
            }
            return new TransactionBuilder(model);
        }

        public static ITransactionBuilder Create
        {
            get => new TransactionBuilder();
        }

        public ITransactionBuilder SetAuxData(IAuxiliaryDataBuilder auxDataBuilder)
        {
            _model.AuxiliaryData = auxDataBuilder.Build();
            return this;
        }

        public ITransactionBuilder SetBody(ITransactionBodyBuilder bodyBuilder)
        {
            _model.TransactionBody = bodyBuilder.Build();
            return this;
        }

        public ITransactionBuilder SetWitnesses(ITransactionWitnessSetBuilder witnessBuilder)
        {
            _model.TransactionWitnessSet = witnessBuilder.Build();
            return this;
        }
    }
}
