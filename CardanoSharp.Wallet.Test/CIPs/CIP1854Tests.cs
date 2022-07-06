using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.TransactionBuilding;
using CardanoSharp.Wallet.Utilities;
using Xunit;

namespace CardanoSharp.Wallet.Test.CIPs;

public class CIP1854Tests
{
    //this is the same as a MultiSig wallet using enterprise addresses 
    [Theory]
    [InlineData("script1cpqa5wphg30n29lfptpm7959hnthm2lfkta0rp2g4cxu7l7tk6q", "addr_test1wrqyrk3cxaz97dghay9v80cksk7dwldtaxe04uv9fzhqmnc7c66vh")]
    public void SharedEnterpriseScriptAddressTest(string controlScriptEncoded, string expectedBechAddress)
    {
        // Create two wallets for multisig
        var mnemonic1 = GetActor1();
        var mnemonic2 = GetActor2();

        PrivateKey rootKey1 = mnemonic1.GetRootKey();
        PrivateKey rootKey2 = mnemonic2.GetRootKey();

        // Establish path
        string paymentPath = $"m/1854'/1815'/0'/0/0";

        // Generate payment keys
        var paymentPrv1 = rootKey1.Derive(paymentPath);
        var paymentPub1 = paymentPrv1.GetPublicKey(false);
        var paymentPrv2 = rootKey2.Derive(paymentPath);
        var paymentPub2 = paymentPrv2.GetPublicKey(false);

        // Generate payment hashes
        var paymentHash1 = HashUtility.Blake2b224(paymentPub1.Key);
        var paymentHash2 = HashUtility.Blake2b224(paymentPub2.Key);

        // Create a Policy Script with a type of Script Any
        var policyScript = ScriptAnyBuilder.Create
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(paymentHash1))
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(paymentHash2))
            .Build();

        // Generate the Policy Id
        var policyId = policyScript.GetPolicyId();
        var controlScriptBytes = Bech32.Decode(controlScriptEncoded, out _, out _);
        Assert.Equal(controlScriptBytes, policyId);
        
        //Generate Address
        var actualAddress = new AddressService().GetEnterpriseScriptAddress(policyId, NetworkType.Testnet);
        var expectedAddress = new Address(expectedBechAddress);
        Assert.Equal(actualAddress.ToString(), expectedAddress.ToString());
    }

    public Mnemonic GetActor1() =>
        new MnemonicService().Restore("scale fiction sadness render fun system hunt skull awake neither quick uncle grab grid credit");

    public Mnemonic GetActor2() =>
        new MnemonicService().Restore("harsh absorb lazy resist elephant social carry roof remember picture merry enlist regret major practice");
}