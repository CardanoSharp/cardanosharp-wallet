using CardanoSharp.Wallet.TransactionBuilding;
using CardanoSharp.Wallet.Extensions;
using Xunit;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.Test
{
    public class TransactionOutputTests
    {
        private readonly IMnemonicService _keyService;

        public TransactionOutputTests()
        {
            _keyService = new MnemonicService();
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

        [Fact]
        public void CalculateMinUtxoTest_TestBundles_ShouldMatchValue()
        {
            var tokenBundle1 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010203".HexToByteArray(), 60)
                .AddToken(getGenesisPolicyId(), "00010204".HexToByteArray(), 240);

            Assert.Equal((ulong)1172320, tokenBundle1.Build().CalculateMinUtxoLovelace());

            var tokenBundle2 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010205".HexToByteArray(), 60)
                .AddToken(getGenesisPolicyId(), "00010206".HexToByteArray(), 120)
                .AddToken(getGenesisPolicyId(), "00010207".HexToByteArray(), 180)
                .AddToken(getGenesisPolicyId(), "00010208".HexToByteArray(), 240);

            Assert.Equal((ulong)1232660, tokenBundle2.Build().CalculateMinUtxoLovelace());
        }

        [Fact]
        public void CalculateMinUtxoTest_CoinOnly_ShouldMatchValue()
        {
            var txOutValue = new TransactionOutputValue() { Coin = 1234567 };
            var txOut = new TransactionOutput() { Value = txOutValue };

            var minValue = txOut.CalculateMinUtxoLovelace();
            Assert.Equal((ulong)1000000, minValue);
        }
    }
}
