using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Derivations;
using CardanoSharp.Wallet.Models.Keys;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Xunit;

namespace CardanoSharp.Wallet.Test
{
    /// <summary>
    /// HD derivation can be seen as a tree with many branches, where keys live at each node and leaf 
    /// such that an entire sub-tree can be recovered from only a parent key (and seemingly, 
    /// the whole tree can be recovered from the root master key).
    /// 
    /// <para>
    ///     References:
    ///     * HD Wallets: https://input-output-hk.github.io/adrestia/docs/key-concepts/hierarchical-deterministic-wallets/
    ///     * Deriving new keys from parent keys: https://input-output-hk.github.io/adrestia/user-guide/Ed25519_BIP.pdf
    /// </para>
    /// </summary>
    public class DerivationTests
    {
        private readonly IKeyService _keyService;
        const string __mnemonic = "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy";

        public DerivationTests()
        {
            _keyService = new KeyService();
        }

        [Theory]
        [InlineData(__mnemonic)]
        public void PathDerivationTest(string words)
        {
            // Arrange
            var mnemonic = new KeyService().Restore(words);
            var rootKey = mnemonic.GetRootKey();
            var testKey = getTestRootKey(mnemonic);

            (var paymentPrv1, var paymentPub1) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            // Act
            // Fluent derivation API
            var derivation = testKey.Derive()                   // IMasterNodeDerivation
                                .Derive(PurposeType.Shelley)    // IPurposeNodeDerivation
                                .Derive(CoinType.Ada)           // ICoinNodeDerivation
                                .Derive(0)                      // IAccountNodeDerivation
                                .Derive(RoleType.ExternalChain) // IRoleNodeDerivation
                                .Derive(0);                     // IIndexNodeDerivation

            // Assert
            Assert.Equal(paymentPrv1.Key, derivation.PrivateKey.Key);
            Assert.Equal(paymentPrv1.Chaincode, derivation.PrivateKey.Chaincode);

            Assert.Equal(paymentPub1.Key, derivation.PublicKey.Key);
            Assert.Equal(paymentPub1.Chaincode, derivation.PublicKey.Chaincode);
        }


        [Theory]
        [InlineData(__mnemonic, "m/1852'")]
        [InlineData(__mnemonic, "m/1852'/1815'")]
        [InlineData(__mnemonic, "m/1852'/1815'/0'")]
        [InlineData(__mnemonic, "m/1852'/1815'/0'/0")]
        [InlineData(__mnemonic, "m/1852'/1815'/0'/0/0")]
        public void PartialPathDerivationTest(string words, string path)
        {
            var depth = path.Split("/").Slice(1).Count(); // strip master node to calc zero based depth

            // create two payment addresses from same root key
            //arrange
            var mnemonic = new KeyService().Restore(words);
            var rootKey = mnemonic.GetRootKey();
            var testKey = getTestRootKey(mnemonic);

            (var prv, var pub) = getKeyPairFromPath(path, rootKey);

            // Act
            // Fluent derivation API
            var master = testKey.Derive();

            var purpose = master.Derive(PurposeType.Shelley);
            var coin = purpose.Derive(CoinType.Ada);
            var account = coin.Derive(0);
            var role = account.Derive(RoleType.ExternalChain);
            var index = role.Derive(0);

            if (depth == 1) AssertDerivedKeys(prv, pub, purpose);
            if (depth == 2) AssertDerivedKeys(prv, pub, coin);
            if (depth == 3) AssertDerivedKeys(prv, pub, account);
            if (depth == 4) AssertDerivedKeys(prv, pub, role);
            if (depth == 5) AssertDerivedKeys(prv, pub, index);
        }

        [Fact]
        public void ImplicitPathDerivationTest()
        {
            var path = "m/1852'/1815'/0'/0/0";

            // Arrange
            var mnemonic = new KeyService().Restore(__mnemonic);
            var rootKey = mnemonic.GetRootKey();
            var testKey = getTestRootKey(mnemonic);

            (var paymentPrv1, var paymentPub1) = getKeyPairFromPath(path, rootKey);

            // Act
            // Fluent derivation API
            var derivation = testKey.Derive(RoleType.ExternalChain)      // implicit IRoleNodeDerivation
                                    .Derive(0);                          // IIndexNodeDerivation
            var derivation2 = testKey.Derive(RoleType.ExternalChain, 0); // implicit IIndexNodeDerivation
            var derivation3 = testKey.DeriveAccount(0)                   // implicit IAccountNodeDerivation
                                    .Derive(RoleType.ExternalChain)      // implicit IRoleNodeDerivation
                                    .Derive(0);                          // IIndexNodeDerivation

            // Assert
            AssertDerivedKeys(paymentPrv1, paymentPub1, derivation);
            AssertDerivedKeys(paymentPrv1, paymentPub1, derivation2);
            AssertDerivedKeys(paymentPrv1, paymentPub1, derivation3);
        }

        private static void AssertDerivedKeys(PrivateKey prv, PublicKey pub, IPathDerivation derivation)
        {
            // Assert
            Assert.Equal(prv.Key, derivation.PrivateKey.Key);
            Assert.Equal(prv.Chaincode, derivation.PrivateKey.Chaincode);

            Assert.Equal(pub.Key, derivation.PublicKey.Key);
            Assert.Equal(pub.Chaincode, derivation.PublicKey.Chaincode);
        }

        /// <summary>
        /// Getting the key from path as descibed in https://github.com/cardano-foundation/CIPs/blob/master/CIP-1852/CIP-1852.md
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rootKey"></param>
        /// <returns></returns>
        private (PrivateKey, PublicKey) getKeyPairFromPath(string path, PrivateKey rootKey)
        {
            var privateKey = rootKey.Derive(path);
            return (privateKey, privateKey.GetPublicKey(false));
        }

        private FluentDerivationPrivateKey getTestRootKey(Mnemonic mnemonic, string password = "")
        {
            var rootKey = KeyDerivation.Pbkdf2(password, mnemonic.Entropy, KeyDerivationPrf.HMACSHA512, 4096, 96);
            rootKey[0] &= 248;
            rootKey[31] &= 31;
            rootKey[31] |= 64;

            return new FluentDerivationPrivateKey(rootKey.Slice(0, 64), rootKey.Slice(64));
        }

        #region AccountDiscovery
        /// <summary>
        /// Path levels
        /// <para>
        /// Cardano wallet defines the following path levels:
        /// m / purpose_H / coin_type_H / account_H / account_type / address_index 
        /// </para>
        /// <para>
        /// purpose_H is set to 1852'
        /// </para>
        /// <para>
        /// coin_type_H is set to 1815'
        /// </para>
        /// <para>
        /// account_H is set to 0'
        /// </para>
        /// <para>
        /// account_type is either
        /// <para>0 to indicate an address on the external chain (public)</para>
        /// <para>1 to indicate an address on the inernal chain (change)</para>
        /// <para>2 to indicate a reward account address (delegation)</para>
        /// </para>
        /// <para>
        /// address_index is either
        /// * 0 if the account_type is 2
        /// * 0 - 2^31 otherwise
        /// </para>
        /// </summary>
        public void AccountDiscoveryTest()
        {
            // arrange
            var walletService = new WalletService();
            var keyService = new KeyService();
            var wordlist = Enums.WordLists.Japanese;
            var mnemonic = keyService.Generate(24, wordlist);

            // act
            var accounts = walletService.DiscoverAccounts(mnemonic);

            // assert
            Assert.NotNull(accounts);
        }
        #endregion
    }
}
