using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Derivations;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Utilities;
using Chaos.NaCl;
using System;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class PrivateKeyExtensions
    {
        const uint MinHardIndex = 0x80000000;

        /// <summary>
        /// Master node derivation
        /// </summary>
        /// <returns></returns>
        public static IMasterNodeDerivation Derive(this PrivateKey privateKey)
        {
            return new MasterNodeDerivation(privateKey);
        }

        /// <summary>
        /// Role node derivation on Account Private Key
        /// </summary>
        /// <param name="role">The role we want to derive</param>
        /// <returns></returns>
        public static IRoleNodeDerivation Derive(this PrivateKey privateKey, RoleType role)
        {
            return new RoleNodeDerivation(privateKey, role);
        }

        public static PrivateKey Encrypt(this PrivateKey privateKey, string password)
        {
            return new PrivateKey(privateKey.Key.Encrypt(password), privateKey.Chaincode.Encrypt(password));
        }

        public static PrivateKey Decrypt(this PrivateKey privateKey, string password)
        {
            return new PrivateKey(privateKey.Key.Decrypt(password), privateKey.Chaincode.Decrypt(password));
        }       
        
        public static PublicKey GetPublicKey(this PrivateKey privateKey, bool withZeroByte = true)
        {
            var sk = new byte[privateKey.Key.Length];
            Buffer.BlockCopy(privateKey.Key, 0, sk, 0, privateKey.Key.Length);
            var pk = Ed25519.GetPublicKey(sk);

            var zero = new byte[] { 0 };

            var buffer = new BigEndianBuffer();
            if (withZeroByte)
                buffer.Write(zero);

            buffer.Write(pk);

            return new PublicKey(
                buffer.ToArray(),
                privateKey.Chaincode);
        }

        public static PrivateKey Derive(this PrivateKey privateKey, string path)
        {
            if (privateKey is null)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }

            if (!Bip32Utility.IsValidPath(path))
                throw new FormatException("Invalid derivation path");

            var segments = path
                .Split('/');

            if (segments[0] == "m") segments = segments.Slice(1);

            PrivateKey newPrivateKey = new PrivateKey(privateKey.Key, privateKey.Chaincode);
            foreach (var segment in segments)
            {
                var isHardened = segment.Contains("'");
                var index = Convert.ToUInt32(segment.Replace("'", ""));

                if (isHardened) index += MinHardIndex;

                newPrivateKey = Bip32Utility.GetChildKeyDerivation(newPrivateKey, index);
            }

            return newPrivateKey;
        }

        public static byte[] Sign(this PrivateKey privateKey, byte[] message)
        {
            var skey = privateKey.Key;

            if (skey.Length == 32)
            {
                skey = Ed25519.ExpandedPrivateKeyFromSeed(skey.Slice(0, 32));
                return Ed25519.Sign(message, skey);
            }
            else
            {
                return Ed25519.SignCrypto(message, skey);
            }
        }
    }
}
