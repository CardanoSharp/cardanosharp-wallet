using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public partial class TransactionBuilder: ABuilder<Transaction>
    {
        public TransactionBuilder()
        {
            _model = new Transaction();
        }

        public TransactionBuilder WithTransactionBody(TransactionBody transactionBody)
        {
            _model.TransactionBody = transactionBody;
            return this;
        }

        public TransactionBuilder WithTransactionWitnessSet(TransactionWitnessSet transactionWitnessSet)
        {
            _model.TransactionWitnessSet = transactionWitnessSet;
            return this;
        }

        public TransactionBuilder WithAuxiliaryData(AuxiliaryData auxiliaryData)
        {
            _model.AuxiliaryData = auxiliaryData;
            return this;
        }
    }
}
