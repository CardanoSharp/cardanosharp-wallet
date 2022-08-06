using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITokenBundleBuilder: IABuilder<Dictionary<byte[], NativeAsset>>
    {
        ITokenBundleBuilder AddToken(byte[] policyId, byte[] asset, long amount);
    }

    public class TokenBundleBuilder: ABuilder<Dictionary<byte[], NativeAsset>>, ITokenBundleBuilder
    {
        private TokenBundleBuilder()
        {
            _model = new Dictionary<byte[], NativeAsset>();
        }

        private TokenBundleBuilder(Dictionary<byte[], NativeAsset> model)
        {
            _model = model;
        }

        public static ITokenBundleBuilder GetBuilder(Dictionary<byte[], NativeAsset> model)
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

        public ITokenBundleBuilder AddToken(byte[] policyId, byte[] asset, long amount)
        {
            var policy = _model.FirstOrDefault(x => x.Key.SequenceEqual(policyId));
            if (policy.Key is null)
            {
                policy = new KeyValuePair<byte[], NativeAsset>(policyId, new NativeAsset());
                _model.Add(policy.Key, policy.Value);
            }

            policy.Value.Token.Add(asset, amount);
            return this;
        }
    }
}
