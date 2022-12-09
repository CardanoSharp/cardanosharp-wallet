using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionOutputBuilder: IABuilder<TransactionOutput>
    {
        ITransactionOutputBuilder SetAddress(byte[] address);

        ITransactionOutputBuilder SetTransactionOutputValue(TransactionOutputValue value);
        ITransactionOutputBuilder SetDatumOption(DatumOption datumOption);
        ITransactionOutputBuilder SetScriptReference(ScriptReference scriptReference);
    }

    public class TransactionOutputBuilder: ABuilder<TransactionOutput>, ITransactionOutputBuilder
    {
        public TransactionOutputBuilder()
        {
            _model = new TransactionOutput();
        }

        private TransactionOutputBuilder(TransactionOutput model)
        {
            _model = model;
        }

        public static ITransactionOutputBuilder GetBuilder(TransactionOutput model)
        {
            if (model == null)
            {
                return new TransactionOutputBuilder();
            }
            return new TransactionOutputBuilder(model);
        }

        public static ITransactionOutputBuilder Create
        {
            get => new TransactionOutputBuilder();
        }     

        public ITransactionOutputBuilder SetAddress(byte[] address)
        {
            _model.Address = address;
            return this;
        }

        public ITransactionOutputBuilder SetTransactionOutputValue(TransactionOutputValue value)
        {
            _model.Value = value;
            return this;
        }

        public ITransactionOutputBuilder SetDatumOption(DatumOption datumOption)
        {
            _model.DatumOption = datumOption;
            return this;
        }

        public ITransactionOutputBuilder SetScriptReference(ScriptReference scriptReference)
        {
            _model.ScriptReference = scriptReference;
           return this;
        }
    }
}
