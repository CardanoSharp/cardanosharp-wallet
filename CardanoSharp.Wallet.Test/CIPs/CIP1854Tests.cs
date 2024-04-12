using System.Threading.Tasks;
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
    [InlineData("script1cpqa5wphg30n29lfptpm7959hnthm2lfkta0rp2g4cxu7l7tk6q", "addr_test1wrqyrk3cxaz97dghay9v80cksk7dwldtaxe04uv9fzhqmnc7c66vh", true)]
    [InlineData("script1cpqa5wphg30n29lfptpm7959hnthm2lfkta0rp2g4cxu7l7tk6q", "addr_test1wrqyrk3cxaz97dghay9v80cksk7dwldtaxe04uv9fzhqmnc7c66vh", false)]
    public void SharedEnterpriseScriptAddressTest(string controlScriptEncoded, string expectedBechAddress, bool useFluentApi)
    {
        // Create two wallets for multisig
        var mnemonic1 = GetActor1();
        var mnemonic2 = GetActor2();

        PublicKey paymentPub1, paymentPub2;
        if (useFluentApi)
        {
            var paymentNode1 = mnemonic1.GetMasterNode()
                .Derive(PurposeType.MultiSig)
                .Derive(CoinType.Ada)
                .Derive(0)
                .Derive(RoleType.ExternalChain)
                .Derive(0);
            paymentNode1.SetPublicKey();
            var paymentNode2 = mnemonic2.GetMasterNode()
                .Derive(PurposeType.MultiSig)
                .Derive(CoinType.Ada)
                .Derive(0)
                .Derive(RoleType.ExternalChain)
                .Derive(0);
            paymentNode2.SetPublicKey();

            paymentPub1 = paymentNode1.PublicKey;
            paymentPub2 = paymentNode2.PublicKey;
        }
        else
        {
            // Get Root Keys
            PrivateKey rootKey1 = mnemonic1.GetRootKey();
            PrivateKey rootKey2 = mnemonic2.GetRootKey();
            
            // Establish path
            string paymentPath = $"m/1854'/1815'/0'/0/0";

            // Generate payment keys
            paymentPub1 = rootKey1.Derive(paymentPath).GetPublicKey(false);
            paymentPub2 = rootKey2.Derive(paymentPath).GetPublicKey(false);
        }

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
        var actualAddress = AddressUtility.GetEnterpriseScriptAddress(policyScript, NetworkType.Testnet);
        var expectedAddress = new Address(expectedBechAddress);
        Assert.Equal(actualAddress.ToString(), expectedAddress.ToString());
    }
    
    //this is the same as a MultiSig wallet using enterprise addresses 
    [Theory]
    [InlineData("script1cpqa5wphg30n29lfptpm7959hnthm2lfkta0rp2g4cxu7l7tk6q", "script12nvu737l7s579tza8wllumf8jtfd25qfx2ngsj5j65kyg36gnem", "addr_test1xrqyrk3cxaz97dghay9v80cksk7dwldtaxe04uv9fzhqmn65m8850hl59832chfmhllx6fuj6t24qzfj56yy4yk493zqrcg27f", true)]
    [InlineData("script1cpqa5wphg30n29lfptpm7959hnthm2lfkta0rp2g4cxu7l7tk6q", "script12nvu737l7s579tza8wllumf8jtfd25qfx2ngsj5j65kyg36gnem", "addr_test1xrqyrk3cxaz97dghay9v80cksk7dwldtaxe04uv9fzhqmn65m8850hl59832chfmhllx6fuj6t24qzfj56yy4yk493zqrcg27f", false)]
    public void SharedDelegationScriptAddressTest(string controlPaymentScriptEncoded, string controlStakeScriptEncoded, string expectedBechAddress, bool useFluentApi)
    {
        // Create two wallets for multisig
        var mnemonic1 = GetActor1();
        var mnemonic2 = GetActor2();

        PublicKey paymentPub1, paymentPub2;
        PublicKey stakePub1, stakePub2;
        if (useFluentApi)
        {
            var paymentNode1 = mnemonic1.GetMasterNode()
                .Derive(PurposeType.MultiSig)
                .Derive(CoinType.Ada)
                .Derive(0)
                .Derive(RoleType.ExternalChain)
                .Derive(0);
            paymentNode1.SetPublicKey();
            var paymentNode2 = mnemonic2.GetMasterNode()
                .Derive(PurposeType.MultiSig)
                .Derive(CoinType.Ada)
                .Derive(0)
                .Derive(RoleType.ExternalChain)
                .Derive(0);
            paymentNode2.SetPublicKey();

            paymentPub1 = paymentNode1.PublicKey;
            paymentPub2 = paymentNode2.PublicKey;
            
            
            var stakeNode1 = mnemonic1.GetMasterNode()
                .Derive(PurposeType.MultiSig)
                .Derive(CoinType.Ada)
                .Derive(0)
                .Derive(RoleType.Staking)
                .Derive(0);
            stakeNode1.SetPublicKey();
            var stakeNode2 = mnemonic2.GetMasterNode()
                .Derive(PurposeType.MultiSig)
                .Derive(CoinType.Ada)
                .Derive(0)
                .Derive(RoleType.Staking)
                .Derive(0);
            stakeNode2.SetPublicKey();

            stakePub1 = stakeNode1.PublicKey;
            stakePub2 = stakeNode2.PublicKey;
        }
        else
        {
            // Get Root Keys
            PrivateKey rootKey1 = mnemonic1.GetRootKey();
            PrivateKey rootKey2 = mnemonic2.GetRootKey();
            
            // Establish path
            string paymentPath = $"m/1854'/1815'/0'/0/0";

            // Generate payment keys
            paymentPub1 = rootKey1.Derive(paymentPath).GetPublicKey(false);
            paymentPub2 = rootKey2.Derive(paymentPath).GetPublicKey(false);
            
            // Establish path
            string stakePath = $"m/1854'/1815'/0'/2/0";

            // Generate payment keys
            stakePub1 = rootKey1.Derive(stakePath).GetPublicKey(false);
            stakePub2 = rootKey2.Derive(stakePath).GetPublicKey(false);
        }

        // Generate payment hashes
        var paymentHash1 = HashUtility.Blake2b224(paymentPub1.Key);
        var paymentHash2 = HashUtility.Blake2b224(paymentPub2.Key);

        // Generate stake hashes
        var stakeHash1 = HashUtility.Blake2b224(stakePub1.Key);
        var stakeHash2 = HashUtility.Blake2b224(stakePub2.Key);

        // Create a Payment Policy Script with a type of Script Any
        var paymentPolicyScript = ScriptAnyBuilder.Create
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(paymentHash1))
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(paymentHash2))
            .Build();
        var paymentPolicyId = paymentPolicyScript.GetPolicyId();
        var bechPaymentPolicyId = Bech32.Encode(paymentPolicyId, "script");

        // Assert Payment Policy Id
        var controlPaymentScriptBytes = Bech32.Decode(controlPaymentScriptEncoded, out _, out _);
        Assert.Equal(controlPaymentScriptBytes, paymentPolicyId);

        // Create a Stake Policy Script with a type of Script Any
        var stakePolicyScript = ScriptAnyBuilder.Create
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(stakeHash1))
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(stakeHash2))
            .Build();
        var statkePolicyId = stakePolicyScript.GetPolicyId();
        var bechStakePolicyId = Bech32.Encode(statkePolicyId, "script");

        // Assert Payment Policy Id
        var controlStakeScriptBytes = Bech32.Decode(controlStakeScriptEncoded, out _, out _);
        Assert.Equal(controlStakeScriptBytes, statkePolicyId);
        
        //Generate Address
        var actualAddress = AddressUtility.GetScriptWithScriptDelegationAddress(paymentPolicyScript, stakePolicyScript, NetworkType.Testnet);
        var expectedAddress = new Address(expectedBechAddress);
        Assert.Equal(actualAddress.ToString(), expectedAddress.ToString());
    }
    
    [Fact]
    public void UnknownScriptTypeTest()
    {
        // Create two wallets for multisig
        var mnemonic1 = GetActor1();
        var mnemonic2 = GetActor2();

        PublicKey paymentPub1, paymentPub2;
        PublicKey stakePub1, stakePub2;

        var paymentNode1 = mnemonic1.GetMasterNode()
            .Derive(PurposeType.MultiSig)
            .Derive(CoinType.Ada)
            .Derive(0)
            .Derive(RoleType.ExternalChain)
            .Derive(0);
        paymentNode1.SetPublicKey();
        var paymentNode2 = mnemonic2.GetMasterNode()
            .Derive(PurposeType.MultiSig)
            .Derive(CoinType.Ada)
            .Derive(0)
            .Derive(RoleType.ExternalChain)
            .Derive(0);
        paymentNode2.SetPublicKey();

        paymentPub1 = paymentNode1.PublicKey;
        paymentPub2 = paymentNode2.PublicKey;
        
        
        var stakeNode1 = mnemonic1.GetMasterNode()
            .Derive(PurposeType.MultiSig)
            .Derive(CoinType.Ada)
            .Derive(0)
            .Derive(RoleType.Staking)
            .Derive(0);
        stakeNode1.SetPublicKey();
        var stakeNode2 = mnemonic2.GetMasterNode()
            .Derive(PurposeType.MultiSig)
            .Derive(CoinType.Ada)
            .Derive(0)
            .Derive(RoleType.Staking)
            .Derive(0);
        stakeNode2.SetPublicKey();

        stakePub1 = stakeNode1.PublicKey;
        stakePub2 = stakeNode2.PublicKey;
        
        // Generate payment hashes
        var paymentHash1 = HashUtility.Blake2b224(paymentPub1.Key);
        var paymentHash2 = HashUtility.Blake2b224(paymentPub2.Key);

        // Generate stake hashes
        var stakeHash1 = HashUtility.Blake2b224(stakePub1.Key);
        var stakeHash2 = HashUtility.Blake2b224(stakePub2.Key);

        // Create a Payment Policy Script with a type of Script Any
        var paymentPolicyScript = ScriptAnyBuilder.Create
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(paymentHash1))
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(paymentHash2))
            .Build();
        var paymentPolicyId = paymentPolicyScript.GetPolicyId();

        // Create a Stake Policy Script with a type of Script Any
        var stakePolicyScript = ScriptAnyBuilder.Create
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(stakeHash1))
            .SetScript(NativeScriptBuilder.Create.SetKeyHash(stakeHash2))
            .Build();
        var statkePolicyId = stakePolicyScript.GetPolicyId();
        
        //Generate Address
        Assert.ThrowsAsync<System.Exception>(() =>
            Task.Run(() => AddressUtility.GetScriptWithScriptDelegationAddress(paymentPolicyId, stakePolicyScript, NetworkType.Testnet)));
        Assert.ThrowsAsync<System.Exception>(() =>
            Task.Run(() => AddressUtility.GetScriptAddress(paymentPolicyScript, statkePolicyId, NetworkType.Testnet)));
    }

    public Mnemonic GetActor1() =>
        new MnemonicService().Restore("scale fiction sadness render fun system hunt skull awake neither quick uncle grab grid credit");

    public Mnemonic GetActor2() =>
        new MnemonicService().Restore("harsh absorb lazy resist elephant social carry roof remember picture merry enlist regret major practice");
}