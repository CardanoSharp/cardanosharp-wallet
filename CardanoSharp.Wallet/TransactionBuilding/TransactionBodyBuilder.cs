using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class TransactionBodyBuilder: ABuilder<TransactionBody>
    {
        public TransactionBodyBuilder()
        {
            _model = new TransactionBody();
        }

        public TransactionBodyBuilder WithTransactionInputs(ICollection<TransactionInput> transactionInputs)
        {
            _model.TransactionInputs = transactionInputs;
            return this;
        }

        public TransactionBodyBuilder WithTransactionOutputs(ICollection<TransactionOutput> transactionOutputs)
        {
            _model.TransactionOutputs = transactionOutputs;
            return this;
        }

        public TransactionBodyBuilder WithFee(uint fee)
        {
            _model.Fee = fee;
            return this;
        }

        public TransactionBodyBuilder WithTtl(uint ttl)
        {
            _model.Ttl = ttl;
            return this;
        }

        public TransactionBodyBuilder WithCertificate(Certificate certificate)
        {
            _model.Certificate = certificate;
            return this;
        }
    }
}
