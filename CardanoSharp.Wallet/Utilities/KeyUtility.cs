using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Utilities
{
    public static class KeyUtility
    {
        public static KeyPair GetKeyPairFromPath(string path, PublicKey key)
        {
            PrivateKey privateKey = null;
            PublicKey publicKey = key.Derive(path);

            return new KeyPair()
            {
                PrivateKey = privateKey,
                PublicKey = publicKey
            };
        }

        public static KeyPair GetKeyPairFromPath(string path, PrivateKey key)
        {
            PrivateKey privateKey = key.Derive(path);
            PublicKey publicKey = privateKey.GetPublicKey(false);

            return new KeyPair()
            {
                PrivateKey = privateKey,
                PublicKey = publicKey
            };
        }
    }
}
