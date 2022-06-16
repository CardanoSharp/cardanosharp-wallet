using Chaos.NaCl;
using System.Security.Cryptography;

namespace CardanoSharp.Wallet.Models.Keys
{
    public class KeyPair
    {
        public PrivateKey PrivateKey { get; private set; }
        public PublicKey PublicKey { get; private set; }

        public KeyPair(PrivateKey privateKey, PublicKey publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }

        public static KeyPair GenerateKeyPair()
        {
            var rngCryptoServiceProvider
                = new RNGCryptoServiceProvider();
            byte[] publicKey;
            byte[] privateKey = new byte[32];

            rngCryptoServiceProvider.GetBytes(privateKey);
            Ed25519.KeyPairFromSeed(out publicKey, out _, privateKey);

            return new KeyPair(
                new PrivateKey(privateKey, new byte[0]),
                new PublicKey(publicKey, new byte[0]));
        }
    }
}
