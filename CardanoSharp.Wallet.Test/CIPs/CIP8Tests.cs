using CardanoSharp.Wallet.CIPs.CIP30.Extensions.Models;
using CardanoSharp.Wallet.CIPs.CIP8;
using CardanoSharp.Wallet.CIPs.CIP8.Models;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;
using CardanoSharp.Wallet.Models.Keys;
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
            var signer = new EdDsaCoseSigner();
            var message = "Hello Cardano!";

            var mnemonic = new MnemonicService().Generate(24);
            var rootKey = mnemonic.GetRootKey();

            // stake 
            var (_, sPub) = rootKey.GetKeyPairFromPath($"m/1852'/1815'/0'/2/0", false);

            // first payment address
            var (aPri, aPub) = rootKey.GetKeyPairFromPath($"m/1852'/1815'/0'/0/0", false);

            var address = AddressUtility.GetBaseAddress(aPub, sPub, NetworkType.Mainnet);

            var coseSign1 = signer.BuildCoseSign1(message.ToBytes(), aPri, address: address.GetBytes());

            var verified = signer.VerifyCoseSign1(coseSign1, aPub, address: address.GetBytes());

            Assert.True(verified);
        }

        [Fact]
        public void ThrowsCoseExceptionWhenBuildingInvalidCoseSign1Message()
        {
            var signer = new EdDsaCoseSigner();
            var message = "Hello Cardano!";

            var mnemonic = new MnemonicService().Generate(24);
            var rootKey = mnemonic.GetRootKey();

            // stake 
            var (_, sPub) = rootKey.GetKeyPairFromPath($"m/1852'/1815'/0'/2/0", false);

            // first payment address
            var (aPri, aPub) = rootKey.GetKeyPairFromPath($"m/1852'/1815'/0'/0/0", false);

            // second payment address
            var (_, a2Pub) = rootKey.GetKeyPairFromPath($"m/1852'/1815'/0'/0/1", false);

            var address = AddressUtility.GetBaseAddress(aPub, sPub, NetworkType.Mainnet);
            var address2 = AddressUtility.GetBaseAddress(a2Pub, sPub, NetworkType.Mainnet);

            var coseSign1 = signer.BuildCoseSign1(message.ToBytes(), aPri, address: address.GetBytes());

            var verified1 = signer.VerifyCoseSign1(coseSign1, a2Pub, address: address.GetBytes());
            var verified2 = signer.VerifyCoseSign1(coseSign1, a2Pub, address: address2.GetBytes());

            Assert.False(verified1);
            Assert.False(verified2);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CompatibleWithCIP30Verification(bool hash)
        {
            var mnemonic = new MnemonicService().Generate(24);

            var rootKey = mnemonic.GetRootKey();

            var (_, sPub) = rootKey.GetKeyPairFromPath("m/1852'/1815'/0'/2/0");
            var (aPriv, _) = rootKey.GetKeyPairFromPath("m/1852'/1815'/0'/0/0");


            var payload = "02708db4-fcd4-48d5-b228-52dd67a0dfd8";

            // CIP8 EdDsaCose signer instance
            var signer = new EdDsaCoseSigner();

            var data = SignDataUtility.SignData(signer, payload, sPub, aPriv, hash: hash);

            // use CIP30 extension to get key
            var coseKey = data.GetCoseKey();

            // use CIP8 coseSign1
            var CIP8_coseSign1 = new CoseSign1(CBORObject.DecodeFromBytes(data.Signature.HexToByteArray()));


            var pubKey = new PublicKey(coseKey.Key, null);

            // verify using CIP8 signer
            var verified_CIP8 = signer.VerifyCoseSign1(CIP8_coseSign1, pubKey);

            // verify using CIP30 
            var verified_CIP30 = data.Verify();


            Assert.True(verified_CIP8);
            Assert.True(verified_CIP30);

        }

        [Theory]
        [InlineData(
            "Lucid",
            "02708db4-fcd4-48d5-b228-52dd67a0dfd8",
            "a4010103272006215820fc4f2524d3787e252ae4b602a09a1aef6a49f95fc16974a1096cb22cefb6dfc4",
            "845846a20127676164647265737358390183b612d7014a6fa718c252b578709adc8f78fb0c7c24d1bd1fa811ac5a30b33efe0365979f90ba3300b233ca81324c103904ea905546a9a7a166686173686564f4582430323730386462342d666364342d343864352d623232382d353264643637613064666438584043688accfc0488f661164b2124f5d061920a9e4aff84c8b25cce796bf15d6a6039035425ce296b00830c9c71e3cdc44e925db1304de46953424c5cf97b37820a")]
        [InlineData(
            "Eternl",
            "Hello Cardano!",
            "a4010103272006215820fc4f2524d3787e252ae4b602a09a1aef6a49f95fc16974a1096cb22cefb6dfc4",
            "845846a20127676164647265737358390183b612d7014a6fa718c252b578709adc8f78fb0c7c24d1bd1fa811ac5a30b33efe0365979f90ba3300b233ca81324c103904ea905546a9a7a166686173686564f44e48656c6c6f2043617264616e6f21584037c3233f4e09dcca86747315f390bcc372f34b55372039533ccc9cb4dcbce5a9939f78ec119ab092cfceaebf88de4e43940704957aea42e5aa8c84908945a40f")]
        public void CompatibleWithWebWallets(string wallet, string payload, string key, string signature)
        {
            // test seed wallet, available online.
            var seed = "scout always message drill gorilla laptop electric decrease fly actor tuition merit clock flush end duck dance treat idle replace bulk total tool assist";
            var mnemonic = new MnemonicService().Restore(seed);

            var rootKey = mnemonic.GetRootKey();

            var data = SignDataUtility.SignData(new EdDsaCoseSigner(), payload, rootKey);

            Assert.Equal(key, data.Key);
            Assert.Equal(signature, data.Signature);
        }

    }
}
