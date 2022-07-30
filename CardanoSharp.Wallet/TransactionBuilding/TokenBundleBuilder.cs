using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITokenBundleBuilder: IABuilder<Dictionary<byte[], NativeAsset<ulong>>>
    {
        ITokenBundleBuilder AddToken(byte[] policyId, byte[] asset, ulong amount);
    }

    public class TokenBundleBuilder: ABuilder<Dictionary<byte[], NativeAsset<ulong>>>, ITokenBundleBuilder
    {
        private TokenBundleBuilder()
        {
            _model = new Dictionary<byte[], NativeAsset<ulong>>();
        }

        private TokenBundleBuilder(Dictionary<byte[], NativeAsset<ulong>> model)
        {
            _model = model;
        }

        public static ITokenBundleBuilder GetBuilder(Dictionary<byte[], NativeAsset<ulong>> model)
        {
            if (model == null)
            {
                return new TokenBundleBuilder();
            }
            return new TokenBundleBuilder(model);
        }

        public static ITokenBundleBuilder Create
        {
            get => new TokenBundleBuilder();
        }

        public ITokenBundleBuilder AddToken(byte[] policyId, byte[] asset, ulong amount)
        {
            var policy = _model.FirstOrDefault(x => x.Key.SequenceEqual(policyId));
            if (policy.Key is null)
            {
                policy = new KeyValuePair<byte[], NativeAsset<ulong>>(policyId, new NativeAsset<ulong>());
                _model.Add(policy.Key, policy.Value);
            }

            policy.Value.Token.Add(asset, amount);
            return this;
        }
    }
}
