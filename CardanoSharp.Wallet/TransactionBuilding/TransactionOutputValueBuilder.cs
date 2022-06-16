using CardanoSharp.Wallet.Models.Transactions;
using System.Collections.Generic;

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
            if (model == null)
            {
                return new TransactionOutputValueBuilder();
            }
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
