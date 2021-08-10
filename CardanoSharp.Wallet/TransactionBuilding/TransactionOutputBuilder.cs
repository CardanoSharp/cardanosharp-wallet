using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class TransactionOutputBuilder: ABuilder<TransactionOutput>
    {
        public TransactionOutputBuilder()
        {
            _model = new TransactionOutput();
        }

        public TransactionOutputBuilder WithAddress(byte[] address)
        {
            _model.Address = address;
            return this;
        }

        public TransactionOutputBuilder WithTransactionOutputValue(TransactionOutputValue value)
        {
            _model.Value = value;
            return this;
        }
    }
}
