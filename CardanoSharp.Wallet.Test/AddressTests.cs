using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CardanoSharp.Wallet.Test
{
    public class AddressTests
    {
        private readonly IAddressService _addressService;
        private readonly IKeyService _keyService;
        const string __mnemonic = "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy";

        public AddressTests()
        {
            _addressService = new AddressService();
            _keyService = new KeyService();
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
            var addrByte = _addressService.GetAddressBytes(addr);
            var addr2 = _addressService.GetAddress(addrByte, prefix);

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
            var addrByte = _addressService.GetAddressBytes(addr);
            var address = new Address(addr);
            var hex = address.ToHexString();

            Assert.Equal(addrByte, address.GetBytes());
            Assert.Equal(prefix, address.Prefix);
            Assert.Equal(prefix == "addr" ? NetworkType.Mainnet : NetworkType.Testnet, address.NetworkType);
            Assert.Equal(AddressType.Base, address.AddressType);

        }

        /// <summary>
        /// Illustrating the fact that addresses generated with paymentPub & stakePub indeed have multiple parts
        /// verifying components of adresses 
        /// inspired by Andrew Westberg (NerdOut) Addresses Video 
        /// https://www.youtube.com/watch?v=NjPf_b9UQNs&t=396)
        /// </summary>
        [Theory]
        [InlineData(__mnemonic, "cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94")]
        public void VerifyRewardAddress(string mnemonic, string stakingAddr)
        {
            // create two payment addresses from same root key
            //arrange
            var entropy = _keyService.Restore(mnemonic);
            var rootKey = _keyService.GetRootKey(entropy);
                        
            ////get payment keys
            (var paymentPrv1, var paymentPub1) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);
            (var paymentPrv2, var paymentPub2) = getKeyPairFromPath("m/1852'/1815'/0'/0/1", rootKey);
            (var paymentPrv3, var paymentPub3) = getKeyPairFromPath("m/1852'/1815'/0'/0/2", rootKey);

            ////get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);
            
            var baseAddr1 = _addressService.GetAddress(paymentPub1, stakePub, NetworkType.Testnet, AddressType.Base);
            var baseAddr2 = _addressService.GetAddress(paymentPub2, stakePub, NetworkType.Testnet, AddressType.Base);
            var baseAddr3 = _addressService.GetAddress(paymentPub3, stakePub, NetworkType.Testnet, AddressType.Base);

            //act
            var hex1 = baseAddr1.ToAddress().ToHexString(); // 0 0 79467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65 cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94
            var hex2 = baseAddr2.ToAddress().ToHexString(); // 0 0 1fd57d18565e3a17cd194f190d349c2b7309eaf70f3f3bf884b0eada cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94
            var hex3 = baseAddr3.ToAddress().ToHexString(); // 0 0 f36b29ceede650f850ee705a3a89ec041e24397d1a0d803d6af7e3f2 cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94
                                                            // ╎ ╎ ╎                                                        ╎
                                                            // ╎ ╎ ╰╌ Payment Address                                       ╰╌ Reward Address 
                                                            // ╎ ╰╌╌╌ NetworkType 0 = Testnet
                                                            // ╰╌╌╌╌╌ AddressType 0 = Base
            // assert
            Assert.EndsWith(stakingAddr, hex1);
            Assert.EndsWith(stakingAddr, hex2);
            Assert.EndsWith(stakingAddr, hex3);

        }

        /// <summary>
        /// Getting the key from path as descibed in https://github.com/cardano-foundation/CIPs/blob/master/CIP-1852/CIP-1852.md
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rootKey"></param>
        /// <returns></returns>
        private (byte[], byte[]) getKeyPairFromPath(string path, (byte[], byte[]) rootKey)
        {
            var privateKey = _keyService.DerivePath(path, rootKey.Item1, rootKey.Item2);
            var publicKey = _keyService.GetPublicKey(privateKey.Item1, false);
            return (privateKey.Item1, publicKey);
        }

    }
}
