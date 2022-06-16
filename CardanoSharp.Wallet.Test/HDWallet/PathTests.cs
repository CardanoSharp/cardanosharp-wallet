using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models;
using System;
using Xunit;

namespace CardanoSharp.Wallet.Test
{
    public class PathTests
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Apostrophe in the path indicates that BIP32 hardened derivation is used. <see href="https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki#path-levels">Path levels</see></param>
        /// <param name="role"></param>
        /// <param name="index"></param>
        [Theory]
        [InlineData("m/44'/1815'/0'/0/0", PurposeType.Byron, CoinType.Ada, 0, RoleType.ExternalChain, 0)]
        [InlineData("m/1852'/1815'/1'/0/0", PurposeType.Shelley, CoinType.Ada, 1, RoleType.ExternalChain, 0)]
        [InlineData("m/1852'/1815'/2'/0/1", PurposeType.Shelley, CoinType.Ada, 2, RoleType.ExternalChain, 1)]
        [InlineData("m/1852'/1815'/0'/1/0", PurposeType.Shelley, CoinType.Ada, 0, RoleType.InternalChain, 0)]
        [InlineData("m/1852'/1815'/0'/1/1", PurposeType.Shelley, CoinType.Ada, 0, RoleType.InternalChain, 1)]
        [InlineData("m/1852'/1815'/0'/2/0", PurposeType.Shelley, CoinType.Ada, 0, RoleType.Staking, 0)]
        public void TestPathFromString(string path, PurposeType purpose, CoinType coin, int accountIx, RoleType role, int index)
        {
            // Arrange

            // Act
            var walletPath = new WalletPath(path);

            // Assert
            Assert.Equal(path, walletPath.ToString());
            Assert.Equal(purpose, walletPath.Purpose);
            Assert.Equal(coin, walletPath.Coin);
            Assert.Equal(accountIx, walletPath.AccountIndex);
            Assert.Equal(role, walletPath.Role);
            Assert.Equal(index, walletPath.Index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Apostrophe in the path indicates that BIP32 hardened derivation is used. <see href="https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki#path-levels">Path levels</see></param>
        /// <param name="role"></param>
        /// <param name="index"></param>
        [Theory]
        [InlineData("m/44'/1815'/0'/0/0", PurposeType.Byron, CoinType.Ada, 0, RoleType.ExternalChain, 0)]
        [InlineData("m/1852'/1815'/1'/0/0", PurposeType.Shelley, CoinType.Ada, 1, RoleType.ExternalChain, 0)]
        [InlineData("m/1852'/1815'/2'/0/1", PurposeType.Shelley, CoinType.Ada, 2, RoleType.ExternalChain, 1)]
        [InlineData("m/1852'/1815'/0'/1/0", PurposeType.Shelley, CoinType.Ada, 0, RoleType.InternalChain, 0)]
        [InlineData("m/1852'/1815'/0'/1/1", PurposeType.Shelley, CoinType.Ada, 0, RoleType.InternalChain, 1)]
        [InlineData("m/1852'/1815'/0'/2/0", PurposeType.Shelley, CoinType.Ada, 0, RoleType.Staking, 0)]
        public void TestPathFromTypes(string path, PurposeType purpose, CoinType coin, int accountIx, RoleType role, int index)
        {
            // Arrange

            // Act
            var walletPath = new WalletPath(purpose,coin,accountIx,role,index);

            // Assert
            Assert.Equal(path, walletPath.ToString());
            Assert.Equal(purpose, walletPath.Purpose);
            Assert.Equal(coin, walletPath.Coin);
            Assert.Equal(accountIx, walletPath.AccountIndex);
            Assert.Equal(role, walletPath.Role);
            Assert.Equal(index, walletPath.Index);
        }

        [Theory]
        [InlineData("m/1852'/1815'/1'", "m/1852'/1815'/1'")]
        [InlineData("1852'/1815'/1'/0/1", "0/1")]
        [InlineData("1815'/1'/0/1", "0/1")]
        [InlineData("1'/0/1", "0/1")]
        [InlineData("/0/1", "0/1")]
        [InlineData("0/1", "0/1")]
        public void NonRootPathsResolveSoftOnly(string p, string expected)
        {
            // Act
            bool success = WalletPath.TryParse(p, out var path);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, path.ToString());
        }

        [Theory]
        [InlineData("m/1852'/1815'")]
        //@TODO fails [InlineData("1852'/1815'/1'")] // dont know if we should allow this
        [InlineData("m 1852' 1815'")]
        [InlineData("1")]
        [InlineData("0")]
        [InlineData("1852'")]
        [InlineData("1815'")]
        [InlineData("1'")]
        [InlineData("m")]
        [InlineData("")]
        public void InvalidPartialPathTest(string path)
        {
            // Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                // Act
                var walletPath = new WalletPath(path);
            });
        }
    }
}
