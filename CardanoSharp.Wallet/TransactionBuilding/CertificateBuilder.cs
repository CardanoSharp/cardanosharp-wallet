using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ICertificateBuilder : IABuilder<Certificate>
    {
        ICertificateBuilder SetStakeRegistration(byte[] stakeRegistration);
        ICertificateBuilder SetStakeDeregistration(byte[] stakeDeregistration);
        ICertificateBuilder SetStakeDelegation(byte[] stakeCredential, byte[] poolHash);
    }

    public class CertificateBuilder: ABuilder<Certificate>, ICertificateBuilder
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
            _model.StakeRegistration = stakeRegistration;
            return this;
        }

        public ICertificateBuilder SetStakeDeregistration(byte[] stakeDeregistration)
        {
            _model.StakeDeregistration = stakeDeregistration;
            return this;
        }

        public ICertificateBuilder SetStakeDelegation(byte[] stakeCredential, byte[] poolHash)
        {
            _model.StakeDelegation = new StakeDelegation()
            {
                StakeCredential = stakeCredential,
                PoolHash = poolHash
            };
            return this;
        }
    }
}
