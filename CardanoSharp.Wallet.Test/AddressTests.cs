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

        public AddressTests()
        {
            _addressService = new AddressService();
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
    }
}
