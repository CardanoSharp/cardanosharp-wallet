using CardanoSharp.Wallet.Models.Keys;
using Chaos.NaCl;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class KeyPairExtensions
    {
        public static byte[] Sign(this KeyPair keyPair, byte[] message)
        {
            var skey = keyPair.PrivateKey.Key;

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

        public static bool Verify(this KeyPair keyPair, byte[] message, byte[] signature)
        {
            return Ed25519.Verify(signature, message, keyPair.PublicKey.Key);
        }
    }
}
