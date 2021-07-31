using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Derivations;
using CardanoSharp.Wallet.Models.Keys;
using System;

namespace CardanoSharp.Wallet.Test
{
    public class FluentDerivationPublicKey : PublicKey
    {
        public FluentDerivationPublicKey(byte[] key, byte[] chaincode) : base(key, chaincode)
        {
        }

        public FluentDerivationPublicKey(PublicKey key) : base(key.Key, key.Chaincode)
        {
        }

        /// <summary>
        /// Master node derivation
        /// </summary>
        /// <returns></returns>
        public IRoleNodeDerivation Derive(RoleType value)
        {
            return new RoleNodeDerivation(new PublicKey(this.Key, this.Chaincode), value);
        }
    }

    public class FluentDerivationPrivateKey : PrivateKey
    {
        public FluentDerivationPrivateKey(byte[] key, byte[] chaincode) : base(key, chaincode)
        {
        }

        public FluentDerivationPrivateKey(PrivateKey key) : base(key.Key, key.Chaincode)
        {
        }

        /// <summary>
        /// Master node derivation
        /// </summary>
        /// <returns></returns>
        public IMasterNodeDerivation Derive()
        {
            return new MasterNodeDerivation(this);
        }

        /// <summary>
        /// Implicit role node derivation for Shelley / Ada
        /// </summary>
        /// <param name="role">The role we want to derive</param>
        /// <returns></returns>
        public IRoleNodeDerivation Derive(RoleType role)
        {
            return DeriveAccount(0).Derive(role);
        }

        /// <summary>
        /// Implicit index node derivation for Shelley / Ada
        /// </summary>
        /// <param name="role">The role we want to derive keys for</param>
        /// <param name="index">The index we want to derive keys for</param>
        /// <returns></returns>
        public IIndexNodeDerivation Derive(RoleType role, int index)
        {
            return Derive(role).Derive(index);
        }

        /// <summary>
        /// Implicit derivation for Shelley / Ada
        /// </summary>
        /// <param name="accountIx">The account index we want to derive keys for</param>
        /// <returns></returns>
        public IAccountNodeDerivation DeriveAccount(int accountIx)
        {
            var master = Derive();
            var purpose = master.Derive();
            var coin = purpose.Derive();
            return coin.Derive(accountIx);
        }
    }
}
