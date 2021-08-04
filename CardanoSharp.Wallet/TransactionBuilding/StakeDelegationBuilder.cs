using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class StakeDelegationBuilder: ABuilder<StakeDelegation>
    {
        public StakeDelegationBuilder()
        {
            _model = new StakeDelegation();
        }

        public StakeDelegationBuilder WithStakeCredential(byte[] stakeCredential)
        {
            _model.StakeCredential = stakeCredential;
            return this;
        }

        public StakeDelegationBuilder WithPoolHash(byte[] poolHash)
        {
            _model.PoolHash = poolHash;
            return this;
        }
    }
}
