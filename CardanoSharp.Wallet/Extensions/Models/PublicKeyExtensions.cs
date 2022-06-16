using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Derivations;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Utilities;
using Chaos.NaCl;
using System;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class PublicKeyExtensions
    {   
        /// <summary>
        /// Master node derivation
        /// </summary>
        /// <returns></returns>
        public static IRoleNodeDerivation Derive(this PublicKey publicKey, RoleType role)
        {
            return new RoleNodeDerivation(publicKey, role);
        }

        public static PublicKey Derive(this PublicKey publicKey, string path)
        {
            if (publicKey is null)
            {
                throw new ArgumentNullException(nameof(publicKey));
            }

            if (!Bip32Utility.IsValidPath(path))
                throw new FormatException("Invalid derivation path");

            var segments = path
                .Split('/');

            PublicKey newpublicKey = new PublicKey(publicKey.Key, publicKey.Chaincode);
            foreach (var segment in segments)
            {
                if (segment.Contains("'"))
                    throw new Exception("Public Keys cannot derive hardened paths");

                var index = Convert.ToUInt32(segment);

                newpublicKey = Bip32Utility.GetChildKeyDerivation(newpublicKey, index);
            }

            return newpublicKey;
        }
        
        public static bool Verify(this PublicKey publicKey, byte[] message, byte[] signature)
        {
            return Ed25519.Verify(signature, message, publicKey.Key);
        }
    }
}
