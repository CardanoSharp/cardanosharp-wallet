using Xunit;
using CardanoSharp.Wallet.Utilities;

namespace CardanoSharp.Wallet.Test
{
    public class AssetLabelTest
    {
        [Fact]
        public void GetAssetLabelHexTest()
        {
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(0), "00000000");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(1), "00001070");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(23), "00017650");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(99), "000632e0");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(222), "000de140");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(333), "0014df10");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(533), "00215410");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(2000), "007d0550");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(4567), "011d7690");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(11111), "02b670b0");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(49328), "0c0b0f40");
            Assert.Equal(AssetLabelUtility.GetAssetLabelHex(65535), "0ffff240");
        }

        [Fact]
        public void GetAssetLabelIntTest()
        {
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("00000000"), 0);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("00001070"), 1);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("00017650"), 23);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("000632e0"), 99);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("000de140"), 222);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("0014df10"), 333);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("00215410"), 533);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("007d0550"), 2000);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("011d7690"), 4567);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("02b670b0"), 11111);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("0c0b0f40"), 49328);
            Assert.Equal(AssetLabelUtility.GetAssetLabelInt("0ffff240"), 65535);
        }
    }
}
