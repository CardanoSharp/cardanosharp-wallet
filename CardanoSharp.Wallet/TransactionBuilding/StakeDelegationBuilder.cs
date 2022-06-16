using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface IStakeDelegationBuilder: IABuilder<StakeDelegation>
    {
        IStakeDelegationBuilder WithStakeCredential(byte[] stakeCredential);

        IStakeDelegationBuilder WithPoolHash(byte[] poolHash);
    }

    public class StakeDelegationBuilder: ABuilder<StakeDelegation>, IStakeDelegationBuilder
    {
        public StakeDelegationBuilder()
        {
            _model = new StakeDelegation();
        }

        private StakeDelegationBuilder(StakeDelegation model)
        {
            _model = model;
        }

        public static IStakeDelegationBuilder GetBuilder(StakeDelegation model)
        {
            if (model == null)
            {
                return new StakeDelegationBuilder();
            }
            return new StakeDelegationBuilder(model);
        }

        public IStakeDelegationBuilder WithStakeCredential(byte[] stakeCredential)
        {
            _model.StakeCredential = stakeCredential;
            return this;
        }

        public IStakeDelegationBuilder WithPoolHash(byte[] poolHash)
        {
            _model.PoolHash = poolHash;
            return this;
        }
    }
}
