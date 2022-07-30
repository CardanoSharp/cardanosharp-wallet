using CardanoSharp.Wallet.Models.Transactions;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface INativeAssetBuilder: IABuilder<NativeAsset>
    {
        INativeAssetBuilder WithToken(Dictionary<byte[], long> token);
    }

    public class NativeAssetBuilder: ABuilder<NativeAsset>, INativeAssetBuilder
    {
        public NativeAssetBuilder()
        {
            _model = new NativeAsset();
        }

        private NativeAssetBuilder(NativeAsset model)
        {
            _model = model;
        }

        public static INativeAssetBuilder GetBuilder(NativeAsset model)
        {
            if (model == null)
            {
                return new NativeAssetBuilder();
            }
            return new NativeAssetBuilder(model);
        }

        public INativeAssetBuilder WithToken(Dictionary<byte[], long> token)
        {
            _model.Token = token;
            return this;
        }
    }
}
