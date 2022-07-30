using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface INativeAssetBuilder: IABuilder<NativeAsset<ulong>>
    {
        INativeAssetBuilder WithToken(Dictionary<byte[], ulong> token);
    }

    public class NativeAssetBuilder: ABuilder<NativeAsset<ulong>>, INativeAssetBuilder
    {
        public NativeAssetBuilder()
        {
            _model = new NativeAsset<ulong>();
        }

        private NativeAssetBuilder(NativeAsset<ulong> model)
        {
            _model = model;
        }

        public static INativeAssetBuilder GetBuilder(NativeAsset<ulong> model)
        {
            if (model == null)
            {
                return new NativeAssetBuilder();
            }
            return new NativeAssetBuilder(model);
        }

        public INativeAssetBuilder WithToken(Dictionary<byte[], ulong> token)
        {
            _model.Token = token;
            return this;
        }
    }
}
