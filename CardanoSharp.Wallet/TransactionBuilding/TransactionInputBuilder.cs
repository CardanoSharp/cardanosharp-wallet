using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class TransactionInputBuilder: ABuilder<TransactionInput>
    {
        public TransactionInputBuilder()
        {
            _model = new TransactionInput();
        }

        public TransactionInputBuilder WithTransactionId(byte[] transactionId)
        {
            _model.TransactionId = transactionId;
            return this;
        }

        public TransactionInputBuilder WithTransactionIndex(uint transactionIndex)
        {
            _model.TransactionIndex = transactionIndex;
            return this;
        }
    }
}
