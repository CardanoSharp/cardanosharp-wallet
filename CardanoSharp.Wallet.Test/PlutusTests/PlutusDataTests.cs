using System.Collections.Generic;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using Xunit;

namespace CardanoSharp.Wallet.Test;

public class PlutusDataTests
{
    [Theory]
    [InlineData(42, "182a")]
    [InlineData(1815 , "190717")]
    public void PlutusDataIntTest(int value, string expectedOutput)
    {
        var data = new PlutusDataInt() { Value = value };
        var serialize =  data.Serialize().ToStringHex();
        Assert.Equal(serialize, expectedOutput);
    }

    [Theory]
    [InlineData(42, "182a")]
    [InlineData(1815 , "190717")]
    public void PlutusDataUIntTest(long value, string expectedOutput)
    {
        var data = new PlutusDataUInt(value);
        var serialize =  data.Serialize().ToStringHex();
        Assert.Equal(serialize, expectedOutput);
    }

    [Theory]
    [InlineData(1, "01")]
    [InlineData(42, "182a")]
    [InlineData(1815 , "190717")]
    public void PlutusDataNIntTest(long value, string expectedOutput)
    {
        var data = new PlutusDataNInt(value);
        var serialize =  data.Serialize().ToStringHex();
        Assert.Equal(serialize, expectedOutput);
    }

    [Theory]
    [InlineData("53776f7264206f6620446f6f6d", "4d53776f7264206f6620446f6f6d")]
    [InlineData("bbf37b90f33b3513b403a4b41263495c90a088746425530dff448b77", "581cbbf37b90f33b3513b403a4b41263495c90a088746425530dff448b77")]
    public void PlutusDataBytesTest(string assetNameHex, string expectedOutput)
    {
        PlutusDataBytes assetKey = new PlutusDataBytes(assetNameHex.HexToByteArray());
        var serialize =  assetKey.Serialize().ToStringHex();
        Assert.Equal(serialize, expectedOutput);
    }

    [Theory]
    [InlineData("53776f7264206f6620446f6f6d", 1, "bbf37b90f33b3513b403a4b41263495c90a088746425530dff448b77", "a1581cbbf37b90f33b3513b403a4b41263495c90a088746425530dff448b77a14d53776f7264206f6620446f6f6d01")]
    [InlineData("426c6f62204d616e", 1, "3b621861995f634530b08bf83573cdfceb8949e91290b596415abc60", "a1581c3b621861995f634530b08bf83573cdfceb8949e91290b596415abc60a148426c6f62204d616e01")]
    public void PlutusDataMapTest(string assetNameHex, ulong quantity, string policyId, string expectedOutput)
    {
        // Create the Asset Map
        PlutusDataBytes assetKey = new PlutusDataBytes(assetNameHex.HexToByteArray());
        PlutusDataInt assetValue = new PlutusDataInt { Value = (int)quantity };

        // Create the Policy Map
        PlutusDataMap policyIdValue = new PlutusDataMap { Value = new Dictionary<IPlutusData, IPlutusData>() { { assetKey, assetValue } } };
        PlutusDataBytes policyIdKey = new PlutusDataBytes(policyId.HexToByteArray());
        PlutusDataMap policyIdMap = new PlutusDataMap() { Value = new Dictionary<IPlutusData, IPlutusData>() { { policyIdKey, policyIdValue } } };
        var serialize =  policyIdMap.Serialize().ToStringHex();
        Assert.Equal(serialize, expectedOutput);
    }

    [Theory]
    [InlineData("53776f7264206f6620446f6f6d", "bbf37b90f33b3513b403a4b41263495c90a088746425530dff448b77", "824d53776f7264206f6620446f6f6d581cbbf37b90f33b3513b403a4b41263495c90a088746425530dff448b77")]
    [InlineData("426c6f62204d616e", "3b621861995f634530b08bf83573cdfceb8949e91290b596415abc60", "8248426c6f62204d616e581c3b621861995f634530b08bf83573cdfceb8949e91290b596415abc60")]
    public void PlutusDataArrayTest(string assetNameHex, string policyId, string expectedOutput)
    {
        IPlutusData[] plutusDatas = new IPlutusData[]
        {
            new PlutusDataBytes(assetNameHex.HexToByteArray()),
            new PlutusDataBytes(policyId.HexToByteArray())
        };
        PlutusDataArray plutusArray = new PlutusDataArray() { Value = plutusDatas };
        var serialize =  plutusArray.Serialize().ToStringHex();
        Assert.Equal(serialize, expectedOutput);
    }

    [Theory]
    [InlineData("53776f7264206f6620446f6f6d", "bbf37b90f33b3513b403a4b41263495c90a088746425530dff448b77", 1, "d87a824d53776f7264206f6620446f6f6d581cbbf37b90f33b3513b403a4b41263495c90a088746425530dff448b77")]
    [InlineData("426c6f62204d616e", "3b621861995f634530b08bf83573cdfceb8949e91290b596415abc60", 1, "d87a8248426c6f62204d616e581c3b621861995f634530b08bf83573cdfceb8949e91290b596415abc60")]
    public void PlutusDataConstrTest(string assetNameHex, string policyId, int alternative, string expectedOutput)
    {
        IPlutusData[] iPlutusUserNftRequiredArray = new IPlutusData[]
        {
            new PlutusDataBytes(assetNameHex.HexToByteArray()),
            new PlutusDataBytes(policyId.HexToByteArray())
        };
        PlutusDataArray plutusUserNftRequiredArray = new PlutusDataArray() { Value = iPlutusUserNftRequiredArray };
        PlutusDataConstr iUserNftRequired = new PlutusDataConstr() { Value = plutusUserNftRequiredArray, Alternative = alternative };
        var serialize =  iUserNftRequired.Serialize().ToStringHex();
        Assert.Equal(serialize, expectedOutput);
    }
}