using CardanoSharp.Wallet.Models.Derivations;
using CardanoSharp.Wallet.Models.Keys;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class MnemonicExtensions
    {
        public static PrivateKey GetRootKey(this Mnemonic mnemonic, string password = "")
        {
            var rootKey = KeyDerivation.Pbkdf2(password, mnemonic.Entropy, KeyDerivationPrf.HMACSHA512, 4096, 96);
            rootKey[0] &= 248;
            rootKey[31] &= 31;
            rootKey[31] |= 64;

            return new PrivateKey(rootKey.Slice(0, 64), rootKey.Slice(64));
        }
        public static MasterNodeDerivation GetMasterNode(this Mnemonic mnemonic, string password = "")
        {
            return new MasterNodeDerivation(
                mnemonic.GetRootKey(password));
        }
    }
}
