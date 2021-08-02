using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;

namespace CardanoSharp.Wallet
{
    public interface ITransactionBuilder
    {
        // a read only reference to the Transaction we are building
        Transaction Transaction { get; }

        ITransactionBuilder AddCertificate(Certificate certificate);
        ITransactionBuilder AddMetadata(Metadata metadata); // base type for PoolMetadata
        ITransactionBuilder AddInput(Address address, int index);
        ITransactionBuilder AddOutput(Address address, int amount);
        ITransactionBuilder AddScript(NativeScript script);
        
        Transaction Sign(params PrivateKey[] witnesses);
    }

    public class TransactionBuilder : ITransactionBuilder
    {
        private Transaction _transaction;
        public Transaction Transaction => _transaction;

        public TransactionBuilder()
        {
            _transaction = new Transaction();
        }

        public ITransactionBuilder AddCertificate(Certificate certificate)
        {
            throw new System.NotImplementedException();
        }

        public ITransactionBuilder AddInput(Address address, int index)
        {
            throw new System.NotImplementedException();
        }

        public ITransactionBuilder AddMetadata(Metadata metadata)
        {
            throw new System.NotImplementedException();
        }

        public ITransactionBuilder AddOutput(Address address, int amount)
        {
            throw new System.NotImplementedException();
        }

        public ITransactionBuilder AddScript(NativeScript script)
        {
            throw new System.NotImplementedException();
        }

        public Transaction Sign(params PrivateKey[] witnesses)
        {
            throw new System.NotImplementedException();
        }
    }
}
