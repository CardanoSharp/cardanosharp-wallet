using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
