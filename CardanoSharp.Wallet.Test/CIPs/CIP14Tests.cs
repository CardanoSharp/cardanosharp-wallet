using CardanoSharp.Wallet.CIPs.CIP14.Extensions;
using Xunit;

namespace CardanoSharp.Wallet.Test.CIPs;

public class CIP14Tests
{
    [Theory]
    [InlineData("7eae28af2208be856f7a119668ae52a49b73725e326dc16579dcc373", "asset1rjklcrnsdzqp65wjgrg55sy9723kw09mlgvlc3")]
    [InlineData("7eae28af2208be856f7a119668ae52a49b73725e326dc16579dcc37e", "asset1nl0puwxmhas8fawxp8nx4e2q3wekg969n2auw3")]
    [InlineData("1e349c9bdea19fd6c147626a5260bc44b71635f398b67c59881df209", "asset1uyuxku60yqe57nusqzjx38aan3f2wq6s93f6ea")]
    [InlineData("7eae28af2208be856f7a119668ae52a49b73725e326dc16579dcc373504154415445", "asset13n25uv0yaf5kus35fm2k86cqy60z58d9xmde92")]
    [InlineData("1e349c9bdea19fd6c147626a5260bc44b71635f398b67c59881df209504154415445", "asset1hv4p5tv2a837mzqrst04d0dcptdjmluqvdx9k3")]
    [InlineData("1e349c9bdea19fd6c147626a5260bc44b71635f398b67c59881df2097eae28af2208be856f7a119668ae52a49b73725e326dc16579dcc373", "asset1aqrdypg669jgazruv5ah07nuyqe0wxjhe2el6f")]
    [InlineData("7eae28af2208be856f7a119668ae52a49b73725e326dc16579dcc3731e349c9bdea19fd6c147626a5260bc44b71635f398b67c59881df209", "asset17jd78wukhtrnmjh3fngzasxm8rck0l2r4hhyyt")]
    [InlineData("7eae28af2208be856f7a119668ae52a49b73725e326dc16579dcc3730000000000000000000000000000000000000000000000000000000000000000", "asset1pkpwyknlvul7az0xx8czhl60pyel45rpje4z8w")]
    public void PolicyIdAssetName_to_AssetFingerprints(
        string tokenTypeId,
        string expected)
    {
        var asset = tokenTypeId.ToAsset();
        var actual = asset.GetFingerprint();
        Assert.Equal(expected, actual);
        var fingerprint = tokenTypeId.GetFingerprint();
        Assert.Equal(expected, fingerprint);
    }
}