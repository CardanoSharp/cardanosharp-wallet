using CardanoSharp.Wallet.Models.Transactions;
using System;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ICertificateBuilder : IABuilder<Certificate>
    {
        ICertificateBuilder SetStakeRegistration(byte[] stakeRegistration);

        ICertificateBuilder SetStakeDeregistration(byte[] stakeDeregistration);

        ICertificateBuilder SetStakeDelegation(byte[] stakeCredential, byte[] poolHash);
    }

    public class CertificateBuilder : ABuilder<Certificate>, ICertificateBuilder
    {
        private CertificateBuilder()
        {
            _model = new Certificate();
        }

        private CertificateBuilder(Certificate model)
        {
            _model = model;
        }

        public static ICertificateBuilder GetBuilder(Certificate model)
        {
            if (model == null)
            {
                return new CertificateBuilder();
            }
            return new CertificateBuilder(model);
        }

        public static ICertificateBuilder Create
        {
            get => new CertificateBuilder();
        }

        public ICertificateBuilder SetStakeRegistration(byte[] stakeRegistration)
        {
            if (stakeRegistration == null)
                throw new ArgumentNullException(nameof(stakeRegistration));
            if (stakeRegistration.Length != 28)
                throw new ArgumentException("stake registration should be 28 bits long. do not include header bit.", nameof(stakeRegistration));

            _model.StakeRegistration = stakeRegistration;
            return this;
        }

        public ICertificateBuilder SetStakeDeregistration(byte[] stakeDeregistration)
        {
            if (stakeDeregistration == null)
                throw new ArgumentNullException(nameof(stakeDeregistration));
            if (stakeDeregistration.Length != 28)
                throw new ArgumentException("stake deregistration should be 28 bits long. do not include header bit.", nameof(stakeDeregistration));

            _model.StakeDeregistration = stakeDeregistration;
            return this;
        }

        public ICertificateBuilder SetStakeDelegation(byte[] stakeCredential, byte[] poolHash)
        {
            if (stakeCredential == null)
                throw new ArgumentNullException(nameof(stakeCredential));
            if (stakeCredential.Length != 28)
                throw new ArgumentException("stake credential should be 28 bits long. do not include header bit.", nameof(stakeCredential));
            if (poolHash == null)
                throw new ArgumentNullException(nameof(poolHash));
            if (poolHash.Length != 28)
                throw new ArgumentException("pool hash should be 28 bits long", nameof(poolHash));

            _model.StakeDelegation = new StakeDelegation()
            {
                StakeCredential = stakeCredential,
                PoolHash = poolHash
            };
            return this;
        }
    }
}