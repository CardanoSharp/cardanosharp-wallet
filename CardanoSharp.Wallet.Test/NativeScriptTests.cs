using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.NativeScripts;
using Xunit;

namespace CardanoSharp.Wallet.Test;

public class NativeScriptTests
{
    [Fact]
    public void Should_Serialize_To_CBOR_Correctly_When_NativeScript_Has_A_Nested_Set_Of_Rules()
    {
        var nativeScript = new NativeScript();
        var topLevelScript = new ScriptAny();
        var timeLockedCustodianScript = new ScriptAll();
        timeLockedCustodianScript.NativeScripts.Add(
            new NativeScript
            {
                InvalidAfter = new ScriptInvalidAfter
                {
                    After = 96997186U
                }
            });
        timeLockedCustodianScript.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "ee367222ee4d74903eb182757e90b82b3be306fe8b06c88bb2382def".HexToByteArray()
                }
            });
        var multiPartyMajorityScript = new ScriptNofK { N = 2 };
        multiPartyMajorityScript.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "ee367222ee4d74903eb182757e90b82b3be306fe8b06c88bb2382def".HexToByteArray()
                }
            });
        multiPartyMajorityScript.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "a6d8d1f9371cd2c0966f8ddb8c793b1af7ead91a222da309e200a170".HexToByteArray()
                }
            });
        multiPartyMajorityScript.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "2c69353be387229dbf2be8d8e4a1c46918e00a5ac3c2501a31500851".HexToByteArray()
                }
            });
        topLevelScript.NativeScripts.Add(new NativeScript { ScriptAll = timeLockedCustodianScript });
        topLevelScript.NativeScripts.Add(new NativeScript { ScriptNofK = multiPartyMajorityScript });
        nativeScript.ScriptAny = topLevelScript;

        var cbor = nativeScript.GetCBOR2();

        var expectedCborJson = "[2,[[1,[[5,96997186],[0,\"7jZyIu5NdJA-sYJ1fpC4KzvjBv6LBsiLsjgt7w\"]]],[3,2,[[0,\"7jZyIu5NdJA-sYJ1fpC4KzvjBv6LBsiLsjgt7w\"],[0,\"ptjR-Tcc0sCWb43bjHk7Gvfq2RoiLaMJ4gChcA\"],[0,\"LGk1O-OHIp2_K-jY5KHEaRjgClrDwlAaMVAIUQ\"]]]]]";
        var actualCborJson = cbor.ToJSONString();
        Assert.Equal(expectedCborJson, actualCborJson);
    }

    [Fact]
    public void Should_Serialize_To_CBOR_Correctly_When_ScriptAll_Has_Single_PubKey()
    {
        var nativeScript = new NativeScript();
        var scriptAll = new ScriptAll();
        scriptAll.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "ee367222ee4d74903eb182757e90b82b3be306fe8b06c88bb2382def".HexToByteArray()
                }
            });
        nativeScript.ScriptAll = scriptAll;

        var cbor = nativeScript.GetCBOR2();

        var expectedCborJson = "[1,[[0,\"7jZyIu5NdJA-sYJ1fpC4KzvjBv6LBsiLsjgt7w\"]]]";
        var actualCborJson = cbor.ToJSONString();
        Assert.Equal(expectedCborJson, actualCborJson);
    }

    [Fact]
    public void Should_Serialize_To_CBOR_Correctly_When_ScriptAll_Has_Multiple_Items()
    {
        var nativeScript = new NativeScript();
        var scriptAll = new ScriptAll();
        scriptAll.NativeScripts.Add(
            new NativeScript
            {
                InvalidAfter = new ScriptInvalidAfter
                {
                    After = 96997186U
                }
            });
        scriptAll.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "ee367222ee4d74903eb182757e90b82b3be306fe8b06c88bb2382def".HexToByteArray()
                }
            });
        nativeScript.ScriptAll = scriptAll;

        var cbor = nativeScript.GetCBOR2();

        var expectedCborJson = "[1,[[5,96997186],[0,\"7jZyIu5NdJA-sYJ1fpC4KzvjBv6LBsiLsjgt7w\"]]]";
        var actualCborJson = cbor.ToJSONString();
        Assert.Equal(expectedCborJson, actualCborJson);
    }

    [Fact]
    public void Should_Serialize_To_CBOR_Correctly_When_ScriptAny_Has_Single_PubKey()
    {
        var nativeScript = new NativeScript();
        var scriptAny = new ScriptAny();
        scriptAny.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "ee367222ee4d74903eb182757e90b82b3be306fe8b06c88bb2382def".HexToByteArray()
                }
            });
        nativeScript.ScriptAny = scriptAny;

        var cbor = nativeScript.GetCBOR2();

        var expectedCborJson = "[2,[[0,\"7jZyIu5NdJA-sYJ1fpC4KzvjBv6LBsiLsjgt7w\"]]]";
        var actualCborJson = cbor.ToJSONString();
        Assert.Equal(expectedCborJson, actualCborJson);
    }

    [Fact]
    public void Should_Serialize_To_CBOR_Correctly_When_ScriptAny_Has_Multiple_Items()
    {
        var nativeScript = new NativeScript();
        var scriptAny = new ScriptAny();
        scriptAny.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "ee367222ee4d74903eb182757e90b82b3be306fe8b06c88bb2382def".HexToByteArray()
                }
            });
        scriptAny.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "a6d8d1f9371cd2c0966f8ddb8c793b1af7ead91a222da309e200a170".HexToByteArray()
                }
            });
        nativeScript.ScriptAny = scriptAny;

        var cbor = nativeScript.GetCBOR2();

        var expectedCborJson = "[2,[[0,\"7jZyIu5NdJA-sYJ1fpC4KzvjBv6LBsiLsjgt7w\"],[0,\"ptjR-Tcc0sCWb43bjHk7Gvfq2RoiLaMJ4gChcA\"]]]";
        var actualCborJson = cbor.ToJSONString();
        Assert.Equal(expectedCborJson, actualCborJson);
    }

    [Fact]
    public void Should_Serialize_To_CBOR_Correctly_When_ScriptNofK_Has_Single_PubKey()
    {
        var nativeScript = new NativeScript();
        var scriptNofK = new ScriptNofK { N = 1 };
        scriptNofK.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "ee367222ee4d74903eb182757e90b82b3be306fe8b06c88bb2382def".HexToByteArray()
                }
            });
        nativeScript.ScriptNofK = scriptNofK;

        var cbor = nativeScript.GetCBOR2();

        var expectedCborJson = $"[3,1,[[0,\"7jZyIu5NdJA-sYJ1fpC4KzvjBv6LBsiLsjgt7w\"]]]";
        var actualCborJson = cbor.ToJSONString();
        Assert.Equal(expectedCborJson, actualCborJson);
    }

    [Theory]
    [InlineData(2U)]
    [InlineData(3U)]
    public void Should_Serialize_To_CBOR_Correctly_When_ScriptNofK_Has_Multiple_Items(uint n)
    {
        var nativeScript = new NativeScript();
        var scriptNofK = new ScriptNofK { N = n };
        scriptNofK.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "ee367222ee4d74903eb182757e90b82b3be306fe8b06c88bb2382def".HexToByteArray()
                }
            });
        scriptNofK.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "a6d8d1f9371cd2c0966f8ddb8c793b1af7ead91a222da309e200a170".HexToByteArray()
                }
            });
        scriptNofK.NativeScripts.Add(
            new NativeScript
            {
                ScriptPubKey = new ScriptPubKey
                {
                    KeyHash = "2c69353be387229dbf2be8d8e4a1c46918e00a5ac3c2501a31500851".HexToByteArray()
                }
            });
        nativeScript.ScriptNofK = scriptNofK;

        var cbor = nativeScript.GetCBOR2();

        var expectedCborJson = $"[3,{n},[[0,\"7jZyIu5NdJA-sYJ1fpC4KzvjBv6LBsiLsjgt7w\"],[0,\"ptjR-Tcc0sCWb43bjHk7Gvfq2RoiLaMJ4gChcA\"],[0,\"LGk1O-OHIp2_K-jY5KHEaRjgClrDwlAaMVAIUQ\"]]]";
        var actualCborJson = cbor.ToJSONString();
        Assert.Equal(expectedCborJson, actualCborJson);
    }
}
