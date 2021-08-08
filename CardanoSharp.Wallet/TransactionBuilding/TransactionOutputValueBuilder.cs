using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class TransactionOutputValueBuilder: ABuilder<TransactionOutputValue>
    {
        public TransactionOutputValueBuilder()
        {
            _model = new TransactionOutputValue();
        }

        public TransactionOutputValueBuilder WithCoin(uint coin)
        {
            _model.Coin = coin;
            return this;
        }

        public TransactionOutputValueBuilder WithMultiAsset(Dictionary<byte[], NativeAsset> multiAsset)
        {
            _model.MultiAsset = multiAsset;
            return this;
        }
    }
}
