using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public class CertificateBuilder: ABuilder<Certificate>
    {
        public CertificateBuilder()
        {
            _model = new Certificate();
        }

        public CertificateBuilder WithStakeRegistration(byte[] stakeRegistration)
        {
            _model.StakeRegistration = stakeRegistration;
            return this;
        }

        public CertificateBuilder WithStakeDeregistration(byte[] stakeDeregistration)
        {
            _model.StakeDeregistration = stakeDeregistration;
            return this;
        }

        public CertificateBuilder WithStakeDelegation(StakeDelegation stakeDelegation)
        {
            _model.StakeDelegation = stakeDelegation;
            return this;
        }
    }
}
