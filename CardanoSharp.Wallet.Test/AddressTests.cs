using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Extensions.Models;
using Xunit;
using CardanoSharp.Wallet.Models.Keys;
using System;
using CardanoSharp.Wallet.Extensions;

namespace CardanoSharp.Wallet.Test
{
    public class AddressTests
    {
        private readonly IAddressService _addressService;
        private readonly IMnemonicService _keyService;
        const string __mnemonic = "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy";

        public AddressTests()
        {
            _addressService = new AddressService();
            _keyService = new MnemonicService();
        }

        [Theory]
        //Delegation Addresses    
        [InlineData("addr_test", "addr_test1qz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3jcu5d8ps7zex2k2xt3uqxgjqnnj83ws8lhrn648jjxtwq2ytjqp")]
        [InlineData("addr", "addr1qx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3jcu5d8ps7zex2k2xt3uqxgjqnnj83ws8lhrn648jjxtwqfjkjv7")]
        [InlineData("addr_test", "addr_test1qpu5vlrf4xkxv2qpwngf6cjhtw542ayty80v8dyr49rf5ewvxwdrt70qlcpeeagscasafhffqsxy36t90ldv06wqrk2qum8x5w")]
        [InlineData("addr", "addr1q9u5vlrf4xkxv2qpwngf6cjhtw542ayty80v8dyr49rf5ewvxwdrt70qlcpeeagscasafhffqsxy36t90ldv06wqrk2qld6xc3")]
        [InlineData("addr_test", "addr_test1qqy6nhfyks7wdu3dudslys37v252w2nwhv0fw2nfawemmn8k8ttq8f3gag0h89aepvx3xf69g0l9pf80tqv7cve0l33sw96paj")]
        [InlineData("addr", "addr1qyy6nhfyks7wdu3dudslys37v252w2nwhv0fw2nfawemmn8k8ttq8f3gag0h89aepvx3xf69g0l9pf80tqv7cve0l33sdn8p3d")]
        //Enterprise Addresses
        [InlineData("addr_test", "addr_test1vz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzerspjrlsz")]
        [InlineData("addr", "addr1vx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzers66hrl8")]
        [InlineData("addr_test", "addr_test1vpu5vlrf4xkxv2qpwngf6cjhtw542ayty80v8dyr49rf5eg57c2qv")]
        [InlineData("addr", "addr1v9u5vlrf4xkxv2qpwngf6cjhtw542ayty80v8dyr49rf5eg0kvk0f")]
        [InlineData("addr_test", "addr_test1vqy6nhfyks7wdu3dudslys37v252w2nwhv0fw2nfawemmnqtjtf68")]
        [InlineData("addr", "addr1vyy6nhfyks7wdu3dudslys37v252w2nwhv0fw2nfawemmnqs6l44z")]
        //Reward
        [InlineData("stake_test", "stake_test1uqevw2xnsc0pvn9t9r9c7qryfqfeerchgrlm3ea2nefr9hqp8n5xl")]
        [InlineData("stake", "stake1uyevw2xnsc0pvn9t9r9c7qryfqfeerchgrlm3ea2nefr9hqxdekzz")]
        public void EncodeDecodeTest(string prefix, string addr)
        {
            var addrByte = Bech32.Decode(addr, out _, out _);
            var addr2 = Bech32.Encode(addrByte, prefix);

            Assert.Equal(addr, addr2);
        }

        //28 
        //addr1q8d9pcrn38veygv638ftw0f82gm4h6rmrs599pkr3qfxx073eyjrmj0hnx6xz8emx03l6hszjclm8fmnlaewe4adp7dqsd8pa6
        //28
        [Theory]
        [InlineData("addr", "addr1q8d9pcrn38veygv638ftw0f82gm4h6rmrs599pkr3qfxx073eyjrmj0hnx6xz8emx03l6hszjclm8fmnlaewe4adp7dqsd8pa6")]
        [InlineData("addr_test", "addr_test1qz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3jcu5d8ps7zex2k2xt3uqxgjqnnj83ws8lhrn648jjxtwq2ytjqp")]
        public void FromStringTest(string prefix, string addr)
        {
            var addrByte = Bech32.Decode(addr, out _, out _);
            var address = new Address(addr);
            var hex = address.ToStringHex();

            Assert.Equal(addrByte, address.GetBytes());
            Assert.Equal(prefix, address.Prefix);
            Assert.Equal(prefix == "addr" ? NetworkType.Mainnet : NetworkType.Testnet, address.NetworkType);
            Assert.Equal(AddressType.Base, address.AddressType);

        }

        /// <summary>
        /// Verifies components of adresses
        /// Illustrating the fact that addresses generated with paymentPub & stakePub have multiple parts,
        /// that addresses consist of "header part", "payment address part" and "reward address part"
        /// that "payment address part" for different paths (CIP1852) differ
        /// that "reward address part" for different paths are equal
        /// 
        /// inspired by Andrew Westberg (NerdOut) Addresses Video 
        /// https://www.youtube.com/watch?v=NjPf_b9UQNs&t=396)
        /// 
        /// 0 0 79467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65 cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94
        /// 0 0 1fd57d18565e3a17cd194f190d349c2b7309eaf70f3f3bf884b0eada cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94
        /// 0 0 f36b29ceede650f850ee705a3a89ec041e24397d1a0d803d6af7e3f2 cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94
        /// ╎ ╎ ╎                                                        ╎
        /// ╎ ╎ ╰╌ Payment Address                                       ╰╌ Reward Address
        /// ╎ ╰╌╌╌ NetworkType 0 = Testnet
        /// ╰╌╌╌╌╌ AddressType 0 = Base
        /// </summary>
        [Theory]
        [InlineData(__mnemonic, "cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94")]
        public void VerifyRewardAddress(string words, string stakingAddr)
        {
            // create two payment addresses from same root key
            //arrange
            var mnemonic = _keyService.Restore(words);
            var rootKey = mnemonic.GetRootKey();
            
            ////get payment keys
            (var paymentPrv1, var paymentPub1) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);
            (var paymentPrv2, var paymentPub2) = getKeyPairFromPath("m/1852'/1815'/0'/0/1", rootKey);
            (var paymentPrv3, var paymentPub3) = getKeyPairFromPath("m/1852'/1815'/0'/0/2", rootKey);

            ////get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);
            
            var baseAddr1 = _addressService.GetBaseAddress(paymentPub1, stakePub, NetworkType.Testnet);
            var baseAddr2 = _addressService.GetBaseAddress(paymentPub2, stakePub, NetworkType.Testnet);
            var baseAddr3 = _addressService.GetBaseAddress(paymentPub3, stakePub, NetworkType.Testnet);

            //act
            var hex1 = baseAddr1.ToStringHex(); 
            var hex2 = baseAddr2.ToStringHex();
            var hex3 = baseAddr3.ToStringHex(); 
                                                
            // assert
            Assert.EndsWith(stakingAddr, hex1);
            Assert.EndsWith(stakingAddr, hex2);
            Assert.EndsWith(stakingAddr, hex3);
        }

        [Theory]
        [InlineData("addr_test1qz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3jcu5d8ps7zex2k2xt3uqxgjqnnj83ws8lhrn648jjxtwq2ytjqp", true)]
        [InlineData("addr1qx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3jcu5d8ps7zex2k2xt3uqxgjqnnj83ws8lhrn648jjxtwqfjkjv7", true)]
        [InlineData("addr_fake1qx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3jcu5d8ps7zex2k2xt3uqxgjqnnj83ws8lhrn648jjxtwqfjkjv7", false)]
        public void HasValidNetworkTest(string addr, bool isValidNetwork)
        {
            Assert.True(new Address(addr).HasValidNetwork() == isValidNetwork);
        }

        [Theory]
        [InlineData("addr_test1qqg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jsnk5zw2", "stake_test1uqneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egft0emn")]
        [InlineData("addr_test1qr3ls8ycdxgvlkqzsw2ysk9w2rpdstm208fnpnnsznst0lvalyteh6cvaz0cgqj7q2hprsvtxqp7w6gpf892vch6l5qs6ug90f", "stake_test1uzwlj9umavxw38uyqf0q9ts3cx9nqql8dyq5nj4xvta06qgqp7kfw")]
        [InlineData("addr1q83ls8ycdxgvlkqzsw2ysk9w2rpdstm208fnpnnsznst0lvalyteh6cvaz0cgqj7q2hprsvtxqp7w6gpf892vch6l5qse249rk", "stake1uxwlj9umavxw38uyqf0q9ts3cx9nqql8dyq5nj4xvta06qg8t55dn")]
        [InlineData("addr1qyg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jssqfzz4", "stake1uyneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egwp9mlw")]
        [InlineData("addr1q88nszffkdktfcycgy6pvya42vtuemu2rxnfa5pgr5qf5r6kmsn4xczc49gggavkvgvsx96zl249yvk5r43t0q0ay9xq29tngn", "stake1u9tdcf6nvpv2j5yywktxyxgrzap042jjxt2p6c4hs87jznq3d0dug")]
        public void ExtractRewardAddressTest(string baseAddressBech32, string expectedRewardAddressBech32)
        {
            var baseAddress = new Address(baseAddressBech32);

            var rewardAddress = _addressService.ExtractRewardAddress(baseAddress);

            Assert.Equal(expectedRewardAddressBech32, rewardAddress.ToString());
        }

        [Theory]
        [InlineData("addr_test1vz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzerspjrlsz")]
        [InlineData("addr1v9u5vlrf4xkxv2qpwngf6cjhtw542ayty80v8dyr49rf5eg0kvk0f")]
        [InlineData("stake_test1uqevw2xnsc0pvn9t9r9c7qryfqfeerchgrlm3ea2nefr9hqp8n5xl")]
        [InlineData("stake1uyevw2xnsc0pvn9t9r9c7qryfqfeerchgrlm3ea2nefr9hqxdekzz")]
        public void ExtractRewardAddressInvalidTest(string invalidAddress)
        {
            var nonBaseAddress = new Address(invalidAddress);

            Assert.Throws<ArgumentException>("basePaymentAddress", () => _addressService.ExtractRewardAddress(nonBaseAddress));
        }

        /// <summary>
        /// Getting the key from path as described in https://github.com/cardano-foundation/CIPs/blob/master/CIP-1852/CIP-1852.md
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rootKey"></param>
        /// <returns></returns>
        private (PrivateKey, PublicKey) getKeyPairFromPath(string path, PrivateKey rootKey)
        {
            var privateKey = rootKey.Derive(path);
            return (privateKey, privateKey.GetPublicKey(false));
        }

        [Fact]
        public void EqualsWithByteArrayTest()
        {
            string strAddress = "addr_test1qz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3jcu5d8ps7zex2k2xt3uqxgjqnnj83ws8lhrn648jjxtwq2ytjqp";
            var address = new Address(strAddress);

            //Converting to hex and back to make sure we have a new array
            var byteAddress = address.GetBytes().ToStringHex().HexToByteArray();

            Assert.True(address.Equals(byteAddress));
        }

        [Fact]
        public void EqualsWithAddressTest()
        {
            string addr = "addr_test1qz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3jcu5d8ps7zex2k2xt3uqxgjqnnj83ws8lhrn648jjxtwq2ytjqp";
            Assert.True(new Address(addr).Equals(new Address(addr)));
        }
    }
}
