using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionOutputBuilder: IABuilder<TransactionOutput>
    {
        ITransactionOutputBuilder WithAddress(byte[] address);

        ITransactionOutputBuilder WithTransactionOutputValue(TransactionOutputValue value);
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
            return new TransactionOutputBuilder(model);
        }

        public ITransactionOutputBuilder WithAddress(byte[] address)
        {
            _model.Address = address;
            return this;
        }

        public ITransactionOutputBuilder WithTransactionOutputValue(TransactionOutputValue value)
        {
            _model.Value = value;
            return this;
        }
    }
}
