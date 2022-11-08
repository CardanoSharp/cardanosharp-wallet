using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions;
using System.Collections.Generic;
using Xunit;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Utilities;
using System.IO;
using System.Text.Json;
using System;
using CardanoSharp.Wallet.TransactionBuilding;
using PeterO.Cbor2;
using System.Linq;
using CardanoSharp.Wallet.Extensions.Models.Transactions.TransactionWitnesses;

namespace CardanoSharp.Wallet.Test
{
    public class TransactionTests
    {
        private readonly TransactionSerializer _transactionSerializer;
        private readonly IMnemonicService _keyService;
        private static string __projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        private static DirectoryInfo __dat = new DirectoryInfo(__projectDirectory).CreateSubdirectory("dat");
        private static JsonSerializerOptions __jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };

        public TransactionTests()
        {
            _keyService = new MnemonicService();
            _transactionSerializer = new TransactionSerializer();
            DirectoryInfo dat = new DirectoryInfo(__projectDirectory).CreateSubdirectory("dat");
        }

        [Fact]
        public void DeserializeTransaction()
        {
            //input & output
            var input1TxHash = "98035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d120".HexToByteArray();
            var payment1Addr = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress();
            var payment2Addr = "addr_test1vqah2xrfp8qjp2tldu8wdq38q8c8tegnduae5zrqff3aeec7g467q".ToAddress();

            //witnesses
            var pubKey = new PublicKey("f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee".HexToByteArray(), null);
            var prvKey = new PrivateKey("c660e50315d76a53d80732efda7630cae8885dfb85c46378684b3c6103e1284a".HexToByteArray(), null);
            var pubKeyHash = HashUtility.Blake2b224(pubKey.Key);
            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(pubKey ,prvKey)
                .SetNativeScript(ScriptAllBuilder.Create
                    .SetScript(NativeScriptBuilder.Create.SetKeyHash(pubKeyHash))
                    .SetScript(NativeScriptBuilder.Create.SetInvalidAfter(90000000U)));

            //cert
            var rootKey = getBase15WordWallet();
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);
            var stakeHash = HashUtility.Blake2b224(stakePub.Key);

            //aux
            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1234, new { name = "simple message", nestedObj = new { nestedName = "testing nesting object (de)serialization", nestedArr = new object[] { "first level", new object[] { "second level" } } } });

            //policy info
            var policyVkey = getGenesisTransaction();
            var policyKeyHash = HashUtility.Blake2b224(policyVkey);

            var scriptAllBuilder = ScriptAllBuilder.Create.SetScript(NativeScriptBuilder.Create.SetKeyHash(policyKeyHash));

            var policyScript = scriptAllBuilder.Build();

            var policyId = policyScript.GetPolicyId();

            string mintAssetName = "token";
            long assetAmount = 1;

            var mintAsset = TokenBundleBuilder.Create
                .AddToken(policyId, mintAssetName.ToBytes(), assetAmount);

            var expectedTrans = TransactionBuilder.Create
                .SetBody(TransactionBodyBuilder.Create
                    .AddInput(input1TxHash, 1)
                    .AddOutput(payment1Addr, 7000000)
                    .AddOutput(payment2Addr, 1674895157)
                    .SetFee(171397)
                    .SetTtl(57910820)
                    .SetMint(mintAsset)
                    .SetCertificate(CertificateBuilder.Create
                        .SetStakeRegistration(stakeHash)
                        .SetStakeDeregistration(stakeHash)
                        .SetStakeDelegation(stakeHash, stakeHash))
                )
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();

            var expected = expectedTrans.GetCBOR().EncodeToBytes().ToStringHex();

            //actual
            var bytes = expected.HexToByteArray();
            var transaction = bytes.DeserializeTransaction();
            var actual = transaction.Serialize().ToStringHex();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DeserializeTransactionValues()
        {
            //input & output
            var input1TxHash = "98035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d120".HexToByteArray();
            var payment1Addr = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress();
            var payment2Addr = "addr_test1vqah2xrfp8qjp2tldu8wdq38q8c8tegnduae5zrqff3aeec7g467q".ToAddress();

            //witnesses
            var pubKey = new PublicKey("f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee".HexToByteArray(), null);
            var prvKey = new PrivateKey("c660e50315d76a53d80732efda7630cae8885dfb85c46378684b3c6103e1284a".HexToByteArray(), null);
            var pubKeyHash = HashUtility.Blake2b224(pubKey.Key);
            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(pubKey, prvKey)
                .AddNativeScript(NativeScriptBuilder.Create.SetScriptNofK(
                    3, new[]
                    {
                        NativeScriptBuilder.Create.SetKeyHash(pubKeyHash),
                        NativeScriptBuilder.Create.SetKeyHash(pubKeyHash),
                        NativeScriptBuilder.Create.SetKeyHash(pubKeyHash),
                        NativeScriptBuilder.Create.SetKeyHash(pubKeyHash),
                        NativeScriptBuilder.Create.SetKeyHash(pubKeyHash),
                    }));

            //cert
            var rootKey = getBase15WordWallet();
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);
            var stakeHash = HashUtility.Blake2b224(stakePub.Key);

            //aux
            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1234, new { name = "simple message", nestedObj = new { nestedName = "testing nesting object (de)serialization", nestedArr = new object[] { "first level", new object[] { "second level" } } } });

            //policy info
            var policyVkey = getGenesisTransaction();
            var policyKeyHash = HashUtility.Blake2b224(policyVkey);

            var scriptAllBuilder = ScriptAllBuilder.Create.SetScript(NativeScriptBuilder.Create.SetKeyHash(policyKeyHash));

            var policyScript = scriptAllBuilder.Build();

            var policyId = policyScript.GetPolicyId();

            string mintAssetName = "token";
            long assetAmount = 1;

            var mintAsset = TokenBundleBuilder.Create
                .AddToken(policyId, mintAssetName.ToBytes(), assetAmount);

            var expected = TransactionBuilder.Create
                .SetBody(TransactionBodyBuilder.Create
                    .AddInput(input1TxHash, 1)
                    .AddOutput(payment1Addr, 7000000)
                    .AddOutput(payment2Addr, 1674895157)
                    .SetFee(171397)
                    .SetTtl(57910820)
                    .SetMint(mintAsset)
                    .SetCertificate(CertificateBuilder.Create
                        .SetStakeRegistration(stakeHash)
                        .SetStakeDeregistration(stakeHash)
                        .SetStakeDelegation(stakeHash, stakeHash))
                )
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();

            
            var txBytes = expected.Serialize();
            var actualDeserialisedTx = txBytes.DeserializeTransaction();

            // Assert the TransactionBody values are expected
            // Assert the TransactionBodyInputs values are expected
            List<TransactionInput> actualTransactionInputs = new List<TransactionInput> {
                new TransactionInput { TransactionId = input1TxHash, TransactionIndex = 1 }
            };
            Assert.Equal(expected.TransactionBody.TransactionInputs.Count, actualTransactionInputs.Count);

            int index = 0;
            foreach (TransactionInput expectedTransactionInput in expected.TransactionBody.TransactionInputs)
            {
                TransactionInput actualTransactionInput = actualTransactionInputs[index];
                Assert.Equal(expectedTransactionInput.TransactionId, actualTransactionInput.TransactionId);
                Assert.Equal(expectedTransactionInput.TransactionIndex, actualTransactionInput.TransactionIndex);
                index += 1;
            }

            // Assert the TransactionBodyOutputs values are expected
            List<TransactionOutput> actualTransactionOutputs = new List<TransactionOutput> {
                new TransactionOutput { Address = payment1Addr.GetBytes(), Value = new TransactionOutputValue { Coin = 7000000, MultiAsset = null } },
                new TransactionOutput { Address = payment2Addr.GetBytes(), Value = new TransactionOutputValue { Coin = 1674895157, MultiAsset = null } }
            };
            Assert.Equal(expected.TransactionBody.TransactionOutputs.Count, actualTransactionOutputs.Count);

            index = 0;
            foreach (TransactionOutput expectedTransactionOutput in expected.TransactionBody.TransactionOutputs)
            {
                TransactionOutput actualTransactionOutput = actualTransactionOutputs[index];
                Assert.Equal(expectedTransactionOutput.Address, actualTransactionOutput.Address);
                Assert.Equal(expectedTransactionOutput.Value.Coin, actualTransactionOutput.Value.Coin);
                Assert.Equal(expectedTransactionOutput.Value.MultiAsset, actualTransactionOutput.Value.MultiAsset);
                index += 1;
            }

            Assert.Equal(expected.TransactionBody.Fee.ToString(), 171397.ToString());
            Assert.Equal(expected.TransactionBody.Ttl.ToString(), 57910820.ToString());
            Assert.Equal(expected.TransactionBody.Certificate.GenesisKeyDelegation, null);
            Assert.Equal(expected.TransactionBody.Certificate.MoveInstantaneousRewardsCert, null);
            Assert.Equal(expected.TransactionBody.Certificate.PoolRegistration, null);
            Assert.Equal(expected.TransactionBody.Certificate.PoolRetirement, null);
            Assert.Equal(expected.TransactionBody.Certificate.StakeDelegation.PoolHash, stakeHash);
            Assert.Equal(expected.TransactionBody.Certificate.StakeDelegation.StakeCredential, stakeHash);
            Assert.Equal(expected.TransactionBody.Withdrawls, null);
            Assert.Equal(expected.TransactionBody.Update, null);
            Assert.Equal(expected.TransactionBody.MetadataHash, null);
            Assert.Equal(expected.TransactionBody.TransactionStartInterval, null);
            Assert.Equal(expected.TransactionBody.Mint, mintAsset.Build());

            // Assert the TransactionWitnessSet values are expected
            // Assert the TransactionWitnessSet VKeyWitness values are expected
            List<VKeyWitness> actualVKeyWitnesses = new List<VKeyWitness> {
                new VKeyWitness {
                    VKey = new PublicKey("f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee".HexToByteArray(), null),
                    SKey = new PrivateKey("c660e50315d76a53d80732efda7630cae8885dfb85c46378684b3c6103e1284a".HexToByteArray(), null),
                    Signature = "1f64eb3258b2df07f9c31d026643790b094df75857e56055675732e1e80b2e06d4a304c2d40fb3ecc51d5983ba49bf18a5c32237038559668c6f60655b376507".HexToByteArray()
                }
            };
            Assert.Equal(expected.TransactionWitnessSet.VKeyWitnesses.Count, actualVKeyWitnesses.Count);

            index = 0;
            foreach (VKeyWitness vKeyWitness in expected.TransactionWitnessSet.VKeyWitnesses)
            {
                VKeyWitness actualVKeyWitness = actualVKeyWitnesses[index];
                Assert.Equal(vKeyWitness.VKey.Key, actualVKeyWitness.VKey.Key);
                Assert.Equal(vKeyWitness.VKey.Chaincode, actualVKeyWitness.VKey.Chaincode);
                Assert.Equal(vKeyWitness.SKey.Key, actualVKeyWitness.SKey.Key);
                Assert.Equal(vKeyWitness.SKey.Chaincode, actualVKeyWitness.SKey.Chaincode);
                Assert.Equal(vKeyWitness.Signature, actualVKeyWitness.Signature);
                index += 1;
            }

            // Assert the TransactionWitnessSet NativeScripts values are expected
            Assert.Equal(1, actualDeserialisedTx.TransactionWitnessSet.NativeScripts.Count);
            var expectedScriptNofK = expected.TransactionWitnessSet.NativeScripts.First().ScriptNofK;
            var actualScriptNofK = actualDeserialisedTx.TransactionWitnessSet.NativeScripts.First().ScriptNofK;
            Assert.Equal(expectedScriptNofK.N, actualScriptNofK.N); 
            foreach (var actualNativeScript in actualScriptNofK.NativeScripts)
            {
                Assert.Equal(pubKeyHash.ToStringHex(), actualNativeScript.ScriptPubKey.KeyHash.ToStringHex());
            }

            // Assert the TransactionWitnessSet BootStrapWitnesses values are expected
            Assert.Equal(expected.TransactionWitnessSet.BootStrapWitnesses, null);

            // Assert the IsValid value is expected
            Assert.Equal(expected.IsValid, true);

            // Assert the AuxiliaryData values are expected
            Assert.Equal(expected.AuxiliaryData.List, new List<object>());
            Assert.Equal(JsonSerializer.Serialize(expected.AuxiliaryData.Metadata),
                         JsonSerializer.Serialize(new Dictionary<int, object>
                         {
                            { 1234, new {
                                name = "simple message",
                                nestedObj = new { nestedName = "testing nesting object (de)serialization", nestedArr = new object[] { "first level", new object[] { "second level" } } } }
                            }
                        }
            ));
        }

        [Fact]
        public void DeserializeMultiAssetTransaction()
        {
            //input & output
            var input1TxHash = "98035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d120".HexToByteArray();
            var payment1Addr = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress();

            //witnesses
            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(
                    new PublicKey("f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee".HexToByteArray(), null),
                    new PrivateKey("c660e50315d76a53d80732efda7630cae8885dfb85c46378684b3c6103e1284a".HexToByteArray(), null)
                );

            //cert
            var rootKey = getBase15WordWallet();
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);
            var stakeHash = HashUtility.Blake2b224(stakePub.Key);

            //aux
            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1234, new { name = "simple message", nestedObj = new { nestedName = "testing nesting object (de)serialization", nestedArr = new object[] { "first level", new object[] { "second level" } } } });

            var tokenBundle1 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010203".HexToByteArray(), 60)
                .AddToken(getGenesisPolicyId(), "00010204".HexToByteArray(), 240);

            var expectedTrans = TransactionBuilder.Create
                .SetBody(TransactionBodyBuilder.Create
                    .AddInput(input1TxHash, 1)
                    .AddOutput(payment1Addr, 1, tokenBundle1, OutputPurpose.Spend)
                    .SetFee(171397)
                    .SetTtl(57910820)
                    .SetCertificate(CertificateBuilder.Create
                        .SetStakeRegistration(stakeHash)
                        .SetStakeDeregistration(stakeHash)
                        .SetStakeDelegation(stakeHash, stakeHash))
                )
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();

            var expected = expectedTrans.GetCBOR().EncodeToBytes().ToStringHex();

            //actual
            var bytes = expected.HexToByteArray();
            var transaction = bytes.DeserializeTransaction();
            var actual = transaction.Serialize().ToStringHex();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DeserializeMultiAssetTransactionValues()
        {
            //input & output
            var input1TxHash = "98035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d120".HexToByteArray();
            var payment1Addr = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress();

            //witnesses
            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(
                    new PublicKey("f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee".HexToByteArray(), null),
                    new PrivateKey("c660e50315d76a53d80732efda7630cae8885dfb85c46378684b3c6103e1284a".HexToByteArray(), null)
                );

            //cert
            var rootKey = getBase15WordWallet();
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);
            var stakeHash = HashUtility.Blake2b224(stakePub.Key);

            //aux
            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1234, new { name = "simple message", nestedObj = new { nestedName = "testing nesting object (de)serialization", nestedArr = new object[] { "first level", new object[] { "second level" } } } });

            var tokenBundle1 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010203".HexToByteArray(), 60)
                .AddToken(getGenesisPolicyId(), "00010204".HexToByteArray(), 240);

            var expected = TransactionBuilder.Create
                .SetBody(TransactionBodyBuilder.Create
                    .AddInput(input1TxHash, 1)
                    .AddOutput(payment1Addr, 1, tokenBundle1, OutputPurpose.Spend)
                    .SetFee(171397)
                    .SetTtl(57910820)
                    .SetCertificate(CertificateBuilder.Create
                        .SetStakeRegistration(stakeHash)
                        .SetStakeDeregistration(stakeHash)
                        .SetStakeDelegation(stakeHash, stakeHash))
                )
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();

            // Assert the TransactionBody values are expected
            // Assert the TransactionBodyInputs values are expected
            List<TransactionInput> actualTransactionInputs = new List<TransactionInput> {
                new TransactionInput { TransactionId = input1TxHash, TransactionIndex = 1 }
            };
            Assert.Equal(expected.TransactionBody.TransactionInputs.Count, actualTransactionInputs.Count);

            // There is only 1 transaction input in the expected transaction body
            TransactionInput expectedTransactionInput = expected.TransactionBody.TransactionInputs.First();
            TransactionInput actualTransactionInput = actualTransactionInputs.First();
            Assert.Equal(expectedTransactionInput.TransactionId, actualTransactionInput.TransactionId);
            Assert.Equal(expectedTransactionInput.TransactionIndex, actualTransactionInput.TransactionIndex);

            // Assert the TransactionBodyOutputs values are expected
            Dictionary<byte[], long> nativeAssetToken = new Dictionary<byte[], long> { { "00010203".HexToByteArray(), 60 }, { "00010204".HexToByteArray(), 240 } };
            List<TransactionOutput> actualTransactionOutputs = new List<TransactionOutput> {
                new TransactionOutput { Address = payment1Addr.GetBytes(),
                                        Value = new TransactionOutputValue { Coin = 1, MultiAsset = new Dictionary<byte[], NativeAsset> { { getGenesisPolicyId(), new NativeAsset { Token = nativeAssetToken } } } }
                }
            };
            Assert.Equal(expected.TransactionBody.TransactionOutputs.Count, actualTransactionOutputs.Count);

            // There is only 1 transaction output in the expected transaction body
            TransactionOutput expectedTransactionOutput = expected.TransactionBody.TransactionOutputs.First();
            TransactionOutput actualTransactionOutput = actualTransactionOutputs.First();
            Assert.Equal(expectedTransactionOutput.Address, actualTransactionOutput.Address);
            Assert.Equal(expectedTransactionOutput.Value.Coin, actualTransactionOutput.Value.Coin);
            Assert.Equal(expectedTransactionOutput.Value.MultiAsset.Count, actualTransactionOutput.Value.MultiAsset.Count);
            Assert.Equal(expectedTransactionOutput.Value.MultiAsset.Keys.ToList(), new List<byte[]> { getGenesisPolicyId() });

            // Dictionary keys byte[] are compared by reference not by value
            var expectedKey = expectedTransactionOutput.Value.MultiAsset.Keys.ToList()[0];
            var actualKey = actualTransactionOutput.Value.MultiAsset.Keys.ToList()[0];
            Assert.Equal(expectedKey, getGenesisPolicyId());
            Assert.Equal(actualKey, getGenesisPolicyId());
            Assert.Equal(expectedTransactionOutput.Value.MultiAsset[expectedKey].Token.Count,
                         actualTransactionOutput.Value.MultiAsset[actualKey].Token.Count);

            var expectedTokenKeys = expectedTransactionOutput.Value.MultiAsset[expectedKey].Token.Keys.ToList();
            var token0ExpectedKey = expectedTokenKeys[0];
            var token1ExpectedKey = expectedTokenKeys[1];

            var actualTokenKeys = actualTransactionOutput.Value.MultiAsset[actualKey].Token.Keys.ToList();
            var token0ActualKey = actualTokenKeys[0];
            var token1ActualKey = actualTokenKeys[1];

            Assert.Equal(token0ExpectedKey, "00010203".HexToByteArray());
            Assert.Equal(token0ActualKey, "00010203".HexToByteArray());
            Assert.Equal(token1ExpectedKey, "00010204".HexToByteArray());
            Assert.Equal(token1ActualKey, "00010204".HexToByteArray());
            Assert.Equal(expectedTransactionOutput.Value.MultiAsset[expectedKey].Token[token0ExpectedKey],
                         actualTransactionOutput.Value.MultiAsset[actualKey].Token[token0ActualKey]);
            Assert.Equal(expectedTransactionOutput.Value.MultiAsset[expectedKey].Token[token1ExpectedKey],
                         actualTransactionOutput.Value.MultiAsset[actualKey].Token[token1ActualKey]);

            // Assert the rest of the TransactionBody values are expected
            Assert.Equal(expected.TransactionBody.Fee.ToString(), 171397.ToString());
            Assert.Equal(expected.TransactionBody.Ttl.ToString(), 57910820.ToString());
            Assert.Equal(expected.TransactionBody.Certificate.GenesisKeyDelegation, null);
            Assert.Equal(expected.TransactionBody.Certificate.MoveInstantaneousRewardsCert, null);
            Assert.Equal(expected.TransactionBody.Certificate.PoolRegistration, null);
            Assert.Equal(expected.TransactionBody.Certificate.PoolRetirement, null);
            Assert.Equal(expected.TransactionBody.Certificate.StakeDelegation.PoolHash, stakeHash);
            Assert.Equal(expected.TransactionBody.Certificate.StakeDelegation.StakeCredential, stakeHash);
            Assert.Equal(expected.TransactionBody.Withdrawls, null);
            Assert.Equal(expected.TransactionBody.Update, null);
            Assert.Equal(expected.TransactionBody.MetadataHash, null);
            Assert.Equal(expected.TransactionBody.TransactionStartInterval, null);
            Assert.Equal(expected.TransactionBody.Mint, new Dictionary<byte[], NativeAsset>());

            // Assert the TransactionWitnessSet values are expected
            // Assert the TransactionWitnessSet VKeyWitness values are expected
            List<VKeyWitness> actualVKeyWitnesses = new List<VKeyWitness> {
                new VKeyWitness {
                    VKey = new PublicKey("f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee".HexToByteArray(), null),
                    SKey = new PrivateKey("c660e50315d76a53d80732efda7630cae8885dfb85c46378684b3c6103e1284a".HexToByteArray(), null),
                    Signature = null
                }
            };
            Assert.Equal(expected.TransactionWitnessSet.VKeyWitnesses.Count, actualVKeyWitnesses.Count);

            int index = 0;
            foreach (VKeyWitness vKeyWitness in expected.TransactionWitnessSet.VKeyWitnesses)
            {
                VKeyWitness actualVKeyWitness = actualVKeyWitnesses[index];
                Assert.Equal(vKeyWitness.VKey.Key, actualVKeyWitness.VKey.Key);
                Assert.Equal(vKeyWitness.VKey.Chaincode, actualVKeyWitness.VKey.Chaincode);
                Assert.Equal(vKeyWitness.SKey.Key, actualVKeyWitness.SKey.Key);
                Assert.Equal(vKeyWitness.SKey.Chaincode, actualVKeyWitness.SKey.Chaincode);
                Assert.Equal(vKeyWitness.Signature, actualVKeyWitness.Signature);
                index += 1;
            }

            // Assert the TransactionWitnessSet NativeScripts values are expected
            Assert.Equal(expected.TransactionWitnessSet.NativeScripts, new HashSet<NativeScript>());

            // Assert the TransactionWitnessSet BootStrapWitnesses values are expected
            Assert.Equal(expected.TransactionWitnessSet.BootStrapWitnesses, null);

            // Assert the IsValid value is expected
            Assert.Equal(expected.IsValid, true);

            // Assert the AuxiliaryData values are expected
            Assert.Equal(expected.AuxiliaryData.List, new List<object>());
            Assert.Equal(JsonSerializer.Serialize(expected.AuxiliaryData.Metadata),
                         JsonSerializer.Serialize(new Dictionary<int, object>
                         {
                            { 1234, new {
                                name = "simple message",
                                nestedObj = new { nestedName = "testing nesting object (de)serialization", nestedArr = new object[] { "first level", new object[] { "second level" } } } }
                            }
                        }
            ));
        }

        [Fact]
        public void SerializeTransaction()
        {
            uint fee = 0;
            uint amount = 0;
            var utxo = "98035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d120".HexToByteArray();
            var payment1Addr = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress();
            var payment2Addr = "addr_test1vqah2xrfp8qjp2tldu8wdq38q8c8tegnduae5zrqff3aeec7g467q".ToAddress();
            byte[] expectedCBOR = "84a3008182582098035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d12000018282581d60d0c43926a989c88d5049e61bdebf2a887aca10fa284b9067373ea28f0082581d603b75186909c120a97f6f0ee6822701f075e5136f3b9a08604a63dce7000200a0f5f6".HexToByteArray();

            // Arrange
            var tx = TransactionBuilder.Create
                .SetBody(TransactionBodyBuilder.Create
                    .AddInput(utxo, 0)
                    .AddOutput(payment1Addr, amount)
                     .AddOutput(payment2Addr, amount)
                    .SetFee(fee)
                )
                .Build();

            // Act
            var actual = CBORObject.DecodeFromBytes(tx.Serialize());
            var expected = CBORObject.DecodeFromBytes(expectedCBOR);
            Assert.Equal(expected, actual);
        }

        private static string ReadVectorFile(string vectorId, string fileName)
        {
            string referenceTx;
            FileInfo referenceTxFile = GetVectorFileInfo(vectorId, fileName);
            using (var fs = referenceTxFile.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    referenceTx = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                }
            }

            return referenceTx;
        }

        private static void WriteVectorFile(string vectorId, string json, string fileName)
        {
            FileInfo draftTxFile = GetVectorFileInfo(vectorId, fileName);
            using (var sw = draftTxFile.CreateText())
            {
                sw.WriteLine(json);
                sw.Close();
            }
        }

        private static FileInfo GetVectorFileInfo(string vectorId, string fn)
        {
            return new FileInfo(GetVectorFilePath(vectorId, fn));
        }

        private static string GetVectorFilePath(string vectorId, string fn)
        {
            return Path.Combine(__dat.FullName, vectorId, fn);
        }


        //Replicate Test from Emurgo's Rust Serialization library
        [Fact]
        public void BuildTxWithChange()
        {
            //arrange
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get change keys
            (var changePrv, var changePub) = getKeyPairFromPath("m/1852'/1815'/0'/1/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);
            var changeAddr = AddressUtility.GetBaseAddress(changePub, stakePub, NetworkType.Testnet);

            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(new byte[32], 0)
                .AddOutput(baseAddr, 10)
                .AddOutput(changeAddr, 856488)
                .SetTtl(1000)
                .SetFee(143502)
                .Build();

            //act
            var serialized = transactionBody.Serialize(null);

            //assert
            Assert.Equal("a4008182582000000000000000000000000000000000000000000000000000000000000000000001828258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d940a82583900c05e80bdcf267e7fe7bf4a867afe54a65a3605b32aae830ed07f8e1ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d941a000d11a8021a0002308e031903e8",
                serialized.ToStringHex());
        }

        //Replicate Test from Emurgo's Rust Serialization library
        [Fact]
        public void CreateTxWithChange()
        {
            //arrange
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get change keys
            (var changePrv, var changePub) = getKeyPairFromPath("m/1852'/1815'/0'/1/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);
            var changeAddr = AddressUtility.GetBaseAddress(changePub, stakePub, NetworkType.Testnet);

            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(new byte[32], 0)
                .AddOutput(baseAddr, 10)
                .AddOutput(changeAddr, 856488)
                .SetTtl(1000)
                .SetFee(143502)
                .Build();

            //act
            var serialized = transactionBody.Serialize(null);

            //assert
            Assert.Equal("a4008182582000000000000000000000000000000000000000000000000000000000000000000001828258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d940a82583900c05e80bdcf267e7fe7bf4a867afe54a65a3605b32aae830ed07f8e1ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d941a000d11a8021a0002308e031903e8",
                serialized.ToStringHex());
        }

        //Replicate Test from Emurgo's Rust Serialization library
        [Fact]
        public void MnemonicToTransaction()
        {
            //arrange
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get change keys
            (var changePrv, var changePub) = getKeyPairFromPath("m/1852'/1815'/0'/1/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);

            var bodyBuilder = TransactionBodyBuilder.Create
                .AddInput("3b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b7", 0)
                .AddOutput(baseAddr, 1)
                .SetTtl(10)
                .SetFee(0);

            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(paymentPub, paymentPrv);

            var transaction = TransactionBuilder.Create
                .SetBody(bodyBuilder)
                .SetWitnesses(witnesses)
                .Build();

            //act
            var serializedTx = transaction.Serialize();

            //assert
            Assert.Equal("84a400818258203b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b70001818258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94010200030aa10081825820489ef28ea97f719ee7768645fc74b811c271e5d7ef06c2310854db30158e945d5840e6489d8cdc11ac139158b878251819a31f01644310fa4a4b9c72c2319aa8887f4e299054346c2ad08016e4b8f55684ccae8bddcc5e2137af730acbed5642ff09f5f6",
                serializedTx.ToStringHex());
        }

        [Fact]
        public void CertificateTest()
        {
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get change keys
            (var changePrv, var changePub) = getKeyPairFromPath("m/1852'/1815'/0'/1/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var changeAddr = AddressUtility.GetBaseAddress(changePub, stakePub, NetworkType.Testnet);
            var stakeHash = HashUtility.Blake2b224(stakePub.Key);

            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput(changeAddr, 3786498)
                .SetCertificate(CertificateBuilder.Create
                    .SetStakeRegistration(stakeHash)
                    .SetStakeDelegation(stakeHash, stakeHash))
                .SetTtl(1000)
                .SetFee(213502)
                .Build();

            //act
            var serialized = transactionBody.Serialize(null);

            //assert
            Assert.Equal("a50081825820000000000000000000000000000000000000000000000000000000000000000000018182583900c05e80bdcf267e7fe7bf4a867afe54a65a3605b32aae830ed07f8e1ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d941a0039c702021a000341fe031903e8048282008200581ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d9483028200581ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94581ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94",
                serialized.ToStringHex());
        }

        [Fact]
        public void OneAssetForEachOutputTest()
        {
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get change keys
            (var changePrv, var changePub) = getKeyPairFromPath("m/1852'/1815'/0'/1/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);
            var changeAddr = AddressUtility.GetBaseAddress(changePub, stakePub, NetworkType.Testnet);

            var tokenBundle1 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010203".HexToByteArray(), 60);

            var tokenBundle2 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010203".HexToByteArray(), 240);

            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput(baseAddr, 1, tokenBundle1, OutputPurpose.Spend)
                .AddOutput(changeAddr, 18, tokenBundle2, OutputPurpose.Spend)
                .SetFee(1)
                .Build();

            //act
            var serialized = transactionBody.Serialize(null);

            //assert
            Assert.Equal("a3008282582000000000000000000000000000000000000000000000000000000000000000000082582000000000000000000000000000000000000000000000000000000000000000000001828258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d948201a1581c00000000000000000000000000000000000000000000000000000000a14400010203183c82583900c05e80bdcf267e7fe7bf4a867afe54a65a3605b32aae830ed07f8e1ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d948212a1581c00000000000000000000000000000000000000000000000000000000a1440001020318f00201",
                serialized.ToStringHex());
        }

        [Fact]
        public void EmptyTokenBundleShouldBehaveAsNoAssetTest()
        {
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);

            //This is the baseline
            var withNoTokenBundle = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput(baseAddr, 1)
                .Build();

            //This should do the same
            var withEmptyTokenBundle = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput(baseAddr, 1, TokenBundleBuilder.Create, OutputPurpose.Spend)
                .Build();

            //act
            var withNoTokenBundleSerialized = withNoTokenBundle.Serialize(null);
            var withEmptyTokenBundleSerialized = withEmptyTokenBundle.Serialize(null);

            //assert
            var withNoTokenBundleHex = withNoTokenBundleSerialized.ToStringHex();
            var withEmptyTokenBundleHex = withEmptyTokenBundleSerialized.ToStringHex();

            Assert.Equal(withNoTokenBundleHex, withEmptyTokenBundleHex);
        }


        [Fact]
        public void OnePolicyMultiAssetForOneOutputTest()
        {
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get change keys
            (var changePrv, var changePub) = getKeyPairFromPath("m/1852'/1815'/0'/1/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);
            var changeAddr = AddressUtility.GetBaseAddress(changePub, stakePub, NetworkType.Testnet);

            var tokenBundle1 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010203".HexToByteArray(), 60)
                .AddToken(getGenesisPolicyId(), "00010204".HexToByteArray(), 240);


            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput(baseAddr, 1, tokenBundle1, OutputPurpose.Spend)
                .SetFee(1)
                .Build();

            //act
            var serialized = transactionBody.Serialize(null);

            //assert
            var hex = serialized.ToStringHex();
            Assert.Equal("a3008282582000000000000000000000000000000000000000000000000000000000000000000082582000000000000000000000000000000000000000000000000000000000000000000001818258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d948201a1581c00000000000000000000000000000000000000000000000000000000a24400010203183c440001020418f00201",
                hex);
        }

        [Fact]
        public void MultiplePolicyOneAssetForOneOutputTest()
        {
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get change keys
            (var changePrv, var changePub) = getKeyPairFromPath("m/1852'/1815'/0'/1/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);
            var changeAddr = AddressUtility.GetBaseAddress(changePub, stakePub, NetworkType.Testnet);

            var tokenBundle1 = TokenBundleBuilder.Create
                .AddToken(getGenesisPolicyId(), "00010203".HexToByteArray(), 60)
                .AddToken(getTest1PolicyId(), "00010204".HexToByteArray(), 240);


            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput(baseAddr, 1, tokenBundle1, OutputPurpose.Spend)
                .SetFee(1)
                .Build();

            //act
            var serialized = transactionBody.Serialize(null);


            //assert
            var hex = serialized.ToStringHex();
            Assert.Equal("a3008282582000000000000000000000000000000000000000000000000000000000000000000082582000000000000000000000000000000000000000000000000000000000000000000001818258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d948201a2581c00000000000000000000000000000000000000000000000000000000a14400010203183c581c01010101010101010101010101010101010101010101010101010101a1440001020418f00201",
                hex);
        }

        [Fact]
        public void SimpleTransactionTest()
        {
            //arrange
            var transactionBody = TransactionBodyBuilder.Create
                .AddInput("3b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b7".HexToByteArray(), 0)
                .AddOutput("611c616f1acb460668a9b2f123c80372c2adad3583b9c6cd2b1deeed1c".HexToByteArray(), 1)
                .SetTtl(10)
                .SetFee(94002);

            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(new PublicKey("f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee".HexToByteArray(), null),
                    new PrivateKey("c660e50315d76a53d80732efda7630cae8885dfb85c46378684b3c6103e1284a".HexToByteArray(), null));

            var transaction = TransactionBuilder.Create
                .SetBody(transactionBody)
                .SetWitnesses(witnesses)
                .Build();

            //act
            var serialized = transaction.Serialize();
            var fee = transaction.CalculateFee();
            var isValid = transaction.IsValid;

            //assert
            Assert.Equal("84a400818258203b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b700018182581d611c616f1acb460668a9b2f123c80372c2adad3583b9c6cd2b1deeed1c01021a00016f32030aa10081825820f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee5840fae5de40c94d759ce13bf9886262159c4f26a289fd192e165995b785259e503f6887bf39dfa23a47cf163784c6eee23f61440e749bc1df3c73975f5231aeda0ff5f6",
                serialized.ToStringHex());
            Assert.Equal((uint)163697, fee);
            Assert.Equal(true, isValid);
        }

        [Fact]
        public void MetadataTest()
        {
            //arrange
            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput("00477367D9134E384A25EDD3E23C72735EE6DE6490D39C537A247E1B65D9E5A6498B927F664A2C82343AA6A50CDDE47DE0A2B8C54ECD9C99C2".HexToByteArray(),
                    1000000)
                .SetTtl(10)
                .SetFee(0);

            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(new PublicKey("0f8ad2c7def332bca2f897ef2a1608ee655341227efe7d2284eeb3f94d08d5fa".HexToByteArray(), null),
                    new PrivateKey("501181718c28e401cb77bb31e65e16c125960d225dc615a20d18cce9cd852f4e9af87333cefe80a142e5f270e03d737f6cb3e5e0f27c023d0c4a6380de0a039d".HexToByteArray(), null));

            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1234, new { name = "simple message" });

            var transaction = TransactionBuilder.Create
                .SetBody(transactionBody)
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();

            //act
            var serialized = transaction.Serialize();

            //assert
            Assert.Equal("84a50081825820000000000000000000000000000000000000000000000000000000000000000000018182583900477367d9134e384a25edd3e23c72735ee6de6490d39c537a247e1b65d9e5a6498b927f664a2c82343aa6a50cdde47de0a2b8c54ecd9c99c21a000f42400200030a0758208dc8a798a1da0e2a6df17e66b10a49b5047133dd4daae2686ef1f73369d3fa16a100818258200f8ad2c7def332bca2f897ef2a1608ee655341227efe7d2284eeb3f94d08d5fa584074a7a181addbda26d7974119ac6e3fe35286fb4a6f7a9db573a5e5836808613097256fa2f0284e255cadc566cef96bde750a3ca5cb79a0726349d3424148e000f582a11904d2a1646e616d656e73696d706c65206d65737361676580",
                serialized.ToStringHex());
        }

        [Theory]
        [InlineData(1, 167789)]
        [InlineData(10, 207785)]
        public void MockingWitnesses_MockViaBuilder_Test(int mocks, int expectedFee)
        {
            //arrange
            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput("00477367D9134E384A25EDD3E23C72735EE6DE6490D39C537A247E1B65D9E5A6498B927F664A2C82343AA6A50CDDE47DE0A2B8C54ECD9C99C2".HexToByteArray(),
                    1000000)
                .SetTtl(10)
                .SetFee(100000);

            var witnesses = TransactionWitnessSetBuilder.Create
                .MockVKeyWitness(mocks);

            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1234, new { name = "simple message" });

            var transaction = TransactionBuilder.Create
                .SetBody(transactionBody)
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();

            //act
            var fee = transaction.CalculateFee();
            transaction.TransactionBody.Fee = fee;
            Assert.Equal(expectedFee, (int)fee);
            Assert.NotNull(transaction.TransactionWitnessSet);

            //the functionality that is here would automatically be done if you use 
            //  transaction.CalculateAndSetFee()
            //  but i wanted to test before and after this piece to ensure "RemoveMocks"
            //  did remove the IsMock VKeyWitnesses
            transaction.TransactionWitnessSet.RemoveMocks();
            Assert.Empty(transaction.TransactionWitnessSet.VKeyWitnesses);
            
            //serialize/deserialize transaction to ensure object was built without mocks and has correct fee
            var serializedTx = transaction.Serialize();
            var deserializedTx = serializedTx.DeserializeTransaction();
            Assert.Equal(fee, deserializedTx.TransactionBody.Fee);
            Assert.Null(deserializedTx.TransactionWitnessSet);
        }

        [Theory]
        [InlineData(1, 167789)]
        [InlineData(10, 207785)]
        public void MockingWitnesses_MockViaList_Test(int mocks, int expectedFee)
        {
            //arrange
            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(getGenesisTransaction(), 0)
                .AddOutput("00477367D9134E384A25EDD3E23C72735EE6DE6490D39C537A247E1B65D9E5A6498B927F664A2C82343AA6A50CDDE47DE0A2B8C54ECD9C99C2".HexToByteArray(),
                    1000000)
                .SetTtl(10)
                .SetFee(100000);

            var witnesses = TransactionWitnessSetBuilder.Create;

            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1234, new { name = "simple message" });

            var transaction = TransactionBuilder.Create
                .SetBody(transactionBody)
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();

            //act
            //the functionality that is here would automatically be done if you use 
            //  transaction.CalculateAndSetFee()
            //  but i wanted to test before and after this piece to ensure "CreateMocks"
            //  did correctly create the witnesses
            transaction.TransactionWitnessSet.VKeyWitnesses.CreateMocks(mocks);
            
            var fee = transaction.CalculateFee();
            transaction.TransactionBody.Fee = fee;
            Assert.Equal(expectedFee, (int)fee);
            Assert.NotNull(transaction.TransactionWitnessSet);

            //the functionality that is here would automatically be done if you use 
            //  transaction.CalculateAndSetFee()
            //  but i wanted to test before and after this piece to ensure "RemoveMocks"
            //  did remove the IsMock VKeyWitnesses
            transaction.TransactionWitnessSet.RemoveMocks();
            Assert.Empty(transaction.TransactionWitnessSet.VKeyWitnesses);
            
            //serialize/deserialize transaction to ensure object was built without mocks and has correct fee
            var serializedTx = transaction.Serialize();
            var deserializedTx = serializedTx.DeserializeTransaction();
            Assert.Equal(fee, deserializedTx.TransactionBody.Fee);
            Assert.Null(deserializedTx.TransactionWitnessSet);
        }

        [Fact]
        public void MintingTest()
        {
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            //get delegation address
            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);

            //policy info
            var policySkey = getGenesisTransaction();
            var policyVkey = getGenesisTransaction();
            var policyKeyHash = HashUtility.Blake2b224(policyVkey);

            var scriptAllBuilder = ScriptAllBuilder.Create.SetScript(NativeScriptBuilder.Create.SetKeyHash(policyKeyHash));

            var policyScript = scriptAllBuilder.Build();

            var policyId = policyScript.GetPolicyId();

            uint txInIndex = 0;
            string txInAddr = getGenesisTransaction().ToStringHex();

            string mintAssetName = "token";
            long assetAmount = 1;

            var mintAsset = TokenBundleBuilder.Create
                .AddToken(policyId, mintAssetName.ToBytes(), assetAmount);

            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(txInAddr.HexToByteArray(), txInIndex)
                .AddOutput(baseAddr, 1,  mintAsset, OutputPurpose.Mint)
                .SetMint(mintAsset)
                .SetTtl(1000)
                .SetFee(0);

            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(paymentPub, paymentPrv)
                .AddVKeyWitness(new PublicKey(policyVkey, new byte[0]), new PrivateKey(policySkey, new byte[0]))
                .SetNativeScript(scriptAllBuilder);

            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1337, new { message = "sharp minting test" });

            var transaction = TransactionBuilder.Create
                .SetBody(transactionBody)
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();

            var signedTxStr = transaction.Serialize().ToStringHex();

            //not the best test but logic was derived from creating this mint
            //  https://explorer.cardano-testnet.iohkdev.io/en/transaction?id=1aff3b12c5b9fb96f0cdcd975b58f6ed273a5680f2ff42a02d82fe0041cf8e3d
            Assert.Equal("84a6008182582000000000000000000000000000000000000000000000000000000000000000000001818258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d948201a1581c7b45f5a5758a8880b4a6fb0da6d6ad3b11963d217658c7d23ebc62b4a145746f6b656e010200031903e8075820e0850084789cdd38358caaa60f7c0326e9fa3d7bd9acf53c95e348389740da4809a1581c7b45f5a5758a8880b4a6fb0da6d6ad3b11963d217658c7d23ebc62b4a145746f6b656e01a20082825820489ef28ea97f719ee7768645fc74b811c271e5d7ef06c2310854db30158e945d58402cbcd64d35f229665e0de915da5eed37f5a69c937804f3957c534f7fc405dcd3abe9b308e9e743797c749c1aa8ff26c8298bdfea8a9078617039b1b0edab820682582000000000000000000000000000000000000000000000000000000000000000005840e3818414929fbb7cabda04358ba51076bf9e888339efa2fb0783314fcafa01b5d57840ef2e00b6fb3fa7432fcaaaf4c06581c68b8e0d3df3f6dc27b6474c9e0201818201818200581cf9dca21a6c826ec8acb4cf395cbc24351937bfe6560b2683ab8b415ff582a1190539a1676d657373616765727368617270206d696e74696e67207465737480",
                signedTxStr);
        }

        [Fact]
        public void BurningTest()
        {
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            //get delegation address
            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);

            //policy info
            var policySkey = getGenesisTransaction();
            var policyVkey = getGenesisTransaction();
            var policyKeyHash = HashUtility.Blake2b224(policyVkey);

            var scriptAllBuilder = ScriptAllBuilder.Create.SetScript(NativeScriptBuilder.Create.SetKeyHash(policyKeyHash));

            var policyScript = scriptAllBuilder.Build();

            var policyId = policyScript.GetPolicyId();

            uint txInIndex = 0;
            string txInAddr = getGenesisTransaction().ToStringHex();

            string burnAssetName = "token";
            long assetAmount = -1;

            var burnAsset = TokenBundleBuilder.Create
                .AddToken(policyId, burnAssetName.ToBytes(), assetAmount);

            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(txInAddr.HexToByteArray(), txInIndex)
                .SetMint(burnAsset)
                .SetTtl(1000)
                .SetFee(0);

            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(paymentPub, paymentPrv)
                .AddVKeyWitness(new PublicKey(policyVkey, new byte[0]), new PrivateKey(policySkey, new byte[0]))
                .SetNativeScript(scriptAllBuilder);

            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1337, new { message = "sharp burning test" });

            var transaction = TransactionBuilder.Create
                .SetBody(transactionBody)
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();

            var signedTxStr = transaction.Serialize().ToStringHex();

            Assert.Equal("84a500818258200000000000000000000000000000000000000000000000000000000000000000000200031903e8075820e3ef965b77defa1103e792740dad9b87136ce2b26f215207eedc822a7693918d09a1581c7b45f5a5758a8880b4a6fb0da6d6ad3b11963d217658c7d23ebc62b4a145746f6b656e20a20082825820489ef28ea97f719ee7768645fc74b811c271e5d7ef06c2310854db30158e945d5840871ce64265329d62f1465fa92fb1c43bbb349b392b9c03a0633cd528386a9960f4412e5e60cdbc6d89fb414b874b8231441d72b771081d7f53e6386ee6998a048258200000000000000000000000000000000000000000000000000000000000000000584090392e604b1be1925e4238db75c53fc5da357b14bab8ba9265fd378037f34d17a3a1f6bf9febba10584420ca5cda76bc492486aae9ed70964034a51a2faedf0d01818201818200581cf9dca21a6c826ec8acb4cf395cbc24351937bfe6560b2683ab8b415ff582a1190539a1676d657373616765727368617270206275726e696e67207465737480",
                signedTxStr);
        }

        

        [Fact]
        public void ExplicitMetadataHashTest() {
             var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            //get delegation address
            var baseAddr = AddressUtility.GetBaseAddress(paymentPub, stakePub, NetworkType.Testnet);

            //policy info
            var policySkey = getGenesisTransaction();
            var policyVkey = getGenesisTransaction();
            var policyKeyHash = HashUtility.Blake2b224(policyVkey);

            var scriptAllBuilder = ScriptAllBuilder.Create.SetScript(NativeScriptBuilder.Create.SetKeyHash(policyKeyHash));

            var policyScript = scriptAllBuilder.Build();

            var policyId = policyScript.GetPolicyId();

            uint txInIndex = 0;
            string txInAddr = getGenesisTransaction().ToStringHex();

            string mintAssetName = "token";
            long assetAmount = 1;

            var mintAsset = TokenBundleBuilder.Create
                .AddToken(policyId, mintAssetName.ToBytes(), assetAmount);
                
            var auxData = AuxiliaryDataBuilder.Create
                .AddMetadata(1337, new { message = "sharp minting test" });

            var transactionBody = TransactionBodyBuilder.Create
                .AddInput(txInAddr.HexToByteArray(), txInIndex)
                .AddOutput(baseAddr, 1, mintAsset, OutputPurpose.Mint)
                .SetMint(mintAsset)
                .SetTtl(1000)
                .SetMetadataHash(auxData)
                .SetFee(0);

            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(paymentPub, paymentPrv)
                .AddVKeyWitness(new PublicKey(policyVkey, new byte[0]), new PrivateKey(policySkey, new byte[0]))
                .SetNativeScript(scriptAllBuilder);

            var transaction = TransactionBuilder.Create
                .SetBody(transactionBody)
                .SetWitnesses(witnesses)
                .Build();

            var transaction2 = TransactionBuilder.Create
                .SetBody(transactionBody)
                .SetWitnesses(witnesses)
                .SetAuxData(auxData)
                .Build();
            transaction2.AuxiliaryData = null;

            string metadata_hash = transactionBody.Build().MetadataHash;

            // Serialize and Deserialize Transaction to ensure the metadata hash is properly kept
            var signedTxStr = transaction.Serialize().ToStringHex();
            var signedTx2Str = transaction2.Serialize().ToStringHex();
            Transaction transactionDeserialized = signedTxStr.HexToByteArray().DeserializeTransaction();
            Transaction transaction2Deserialized = signedTx2Str.HexToByteArray().DeserializeTransaction();

            // Asset the metadata hash before and after are equal
            Assert.Equal(metadata_hash, transactionDeserialized.TransactionBody.MetadataHash.ToLower());
            Assert.Equal(metadata_hash, transaction2Deserialized.TransactionBody.MetadataHash.ToLower());
            Assert.Equal(metadata_hash, "e0850084789cdd38358caaa60f7c0326e9fa3d7bd9acf53c95e348389740da48");
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

        private byte[] getGenesisPolicyId()
        {
            var hash = new byte[28];
            for (var i = 0; i < hash.Length; i++)
            {
                hash[i] = 0x00;
            }
            return hash;
        }

        private byte[] getTest1PolicyId()
        {
            var hash = new byte[28];
            for (var i = 0; i < hash.Length; i++)
            {
                hash[i] = 0x01;
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
    }
}
