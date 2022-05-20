using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionOutputValueBuilder: IABuilder<TransactionOutputValue>
    {
        ITransactionOutputValueBuilder WithCoin(uint coin);

        ITransactionOutputValueBuilder WithMultiAsset(Dictionary<byte[], NativeAsset> multiAsset);
    }

    public class TransactionOutputValueBuilder: ABuilder<TransactionOutputValue>, ITransactionOutputValueBuilder
    {
        public TransactionOutputValueBuilder()
        {
            _model = new TransactionOutputValue();
        }

        private TransactionOutputValueBuilder(TransactionOutputValue model)
        {
            _model = model;
        }

        public static ITransactionOutputValueBuilder GetBuilder(TransactionOutputValue model)
        {
            return new TransactionOutputValueBuilder(model);
        }

        public ITransactionOutputValueBuilder WithCoin(uint coin)
        {
            _model.Coin = coin;
            return this;
        }

        public ITransactionOutputValueBuilder WithMultiAsset(Dictionary<byte[], NativeAsset> multiAsset)
        {
            _model.MultiAsset = multiAsset;
            return this;
        }
    }
}
