using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITokenBurnBuilder: IABuilder<Dictionary<byte[], NativeAsset<long>>>
    {
        ITokenBurnBuilder AddToken(byte[] policyId, byte[] asset, long amount);
    }

    public class TokenBurnBuilder: ABuilder<Dictionary<byte[], NativeAsset<long>>>, ITokenBurnBuilder
    {
        private TokenBurnBuilder()
        {
            _model = new Dictionary<byte[], NativeAsset<long>>();
        }

        private TokenBurnBuilder(Dictionary<byte[], NativeAsset<long>> model)
        {
            _model = model;
        }

        public static ITokenBurnBuilder GetBuilder(Dictionary<byte[], NativeAsset<long>> model)
        {
            if (model == null)
            {
                return new TokenBurnBuilder();
            }
            return new TokenBurnBuilder(model);
        }

        public static ITokenBurnBuilder Create
        {
            get => new TokenBurnBuilder();
        }

        public ITokenBurnBuilder AddToken(byte[] policyId, byte[] asset, long amount)
        {
            var policy = _model.FirstOrDefault(x => x.Key.SequenceEqual(policyId));
            if (policy.Key is null)
            {
                policy = new KeyValuePair<byte[], NativeAsset<long>>(policyId, new NativeAsset<long>());
                _model.Add(policy.Key, policy.Value);
            }

            policy.Value.Token.Add(asset, amount);
            return this;
        }
    }
}
