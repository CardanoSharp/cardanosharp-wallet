using CardanoSharp.Wallet.TransactionBuilding;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Extensions.Models.Transactions;

namespace CardanoSharp.Wallet.Test
{
    public class TransactionOutputTests
    {
        private readonly IAddressService _addressService;
        private readonly IMnemonicService _keyService;

        public TransactionOutputTests()
        {
            _keyService = new MnemonicService();
            _addressService = new AddressService();
        }

        private byte[] getGenesisPolicyId()
        {
            var hash = new byte[28];
            for (var i = 0; i < hash.Length; i++)
            {
                hash[i] = 0x00;
            }
            return hash;
        }

        private byte[] getGenesisTransaction()
        {
            var hash = new byte[32];
            for (var i = 0; i < hash.Length; i++)
            {
                hash[i] = 0x00;
            }
            return hash;
        }

        private PrivateKey getBase15WordWallet()
        {
            var words = "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy";
            var mnemonic = _keyService.Restore(words);
            return mnemonic.GetRootKey();
        }

        private (PrivateKey, PublicKey) getKeyPairFromPath(string path, PrivateKey rootKey)
        {
            var privateKey = rootKey.Derive(path);
            return (privateKey, privateKey.GetPublicKey(false));
        }

        [Fact]
        public void CalculateMinUtxoTest_MultiOutput_ShouldMatchCoinValue()
        {
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get change keys
            (var changePrv, var changePub) = getKeyPairFromPath("m/1852'/1815'/0'/1/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var baseAddr = _addressService.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);
            var changeAddr = _addressService.GetBaseAddress(changePub, stakePub, NetworkType.Testnet);

            var tokenBundle1 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010203".HexToByteArray(), 60)
                .AddToken(getGenesisPolicyId(), "00010204".HexToByteArray(), 240);

            var tokenBundle2 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010205".HexToByteArray(), 60)
                .AddToken(getGenesisPolicyId(), "00010206".HexToByteArray(), 120)
                .AddToken(getGenesisPolicyId(), "00010207".HexToByteArray(), 180)
                .AddToken(getGenesisPolicyId(), "00010208".HexToByteArray(), 240);

            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput(baseAddr, 1413762, tokenBundle1)
                .AddOutput(changeAddr, 1551690, tokenBundle2)
                .SetFee(1)
                .Build();

            foreach (var output in transactionBody.TransactionOutputs)
            {
                var minValue = output.CalculateMinUtxoLovelace();
                Assert.Equal(output.Value.Coin, minValue);
            }
        }
    }
}
