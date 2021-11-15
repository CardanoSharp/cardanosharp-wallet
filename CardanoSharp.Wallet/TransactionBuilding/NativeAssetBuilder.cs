using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class NativeAssetBuilder: ABuilder<NativeAsset>
    {
        public NativeAssetBuilder()
        {
            _model = new NativeAsset();
        }

        public NativeAssetBuilder WithToken(Dictionary<byte[], ulong> token)
        {
            _model.Token = token;
            return this;
        }
    }
}
