using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Keys;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
