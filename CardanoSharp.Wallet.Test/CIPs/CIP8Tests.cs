using CardanoSharp.Wallet.CIPs.CIP8.Models;
using CardanoSharp.Wallet.Extensions;
using PeterO.Cbor2;
using Xunit;

namespace CardanoSharp.Wallet.Test.CIPs
{
    public class CIP8Tests
    {
        [Fact]
        public void DecodeCoseSign1Correctly()
        {
            var coseSignMessageHex = "845869a30127045820674d11e432450118d70ea78673d5e31d5cc1aec63de0ff6284784876544be3406761646472657373583901d2eb831c6cad4aba700eb35f86966fbeff19d077954430e32ce65e8da79a3abe84f4ce817fad066acc1435be2ffc6bd7dce2ec1cc6cca6cba166686173686564f44568656c6c6f5840a3b5acd99df5f3b5e4449c5a116078e9c0fcfc126a4d4e2f6a9565f40b0c77474cafd89845e768fae3f6eec0df4575fcfe7094672c8c02169d744b415c617609";
            var coseSignMessageBytes = coseSignMessageHex.HexToByteArray();

            var coseSignCbor = CBORObject.DecodeFromBytes(coseSignMessageBytes);
            var coseSign = new CoseSign1(coseSignCbor);
            Assert.NotNull(coseSign);
        }

        [Fact]
        public void EncodeCoseSign1Correctly()
        {
            
        }

        [Fact]
        public void SignAndVerifyValidCoseSign1Message()
        {

        }

        [Fact]
        public void ThrowsCoseExceptionWhenBuildingInvalidCoseSign1Message()
        {

        }
    }
}
