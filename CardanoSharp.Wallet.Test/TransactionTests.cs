using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions;
using System.Collections.Generic;
using Xunit;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Utilities;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Reflection;
using System;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;

namespace CardanoSharp.Wallet.Test
{
    public class TransactionTests
    {
        private readonly TransactionSerializer _transactionSerializer;
        private readonly IKeyService _keyService;
        private readonly IAddressService _addressService;
        private static string __projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;      
        private static DirectoryInfo __dat = new DirectoryInfo(__projectDirectory).CreateSubdirectory("dat");
        private static JsonSerializerOptions __jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };

        public TransactionTests()
        {
            _keyService = new KeyService();
            _addressService = new AddressService();
            _transactionSerializer = new TransactionSerializer();
            DirectoryInfo dat = new DirectoryInfo(__projectDirectory).CreateSubdirectory("dat");
        }

        [Fact]
        public void DeserializeTransaction()
        {
            
        }

        //[Fact]
        public void SerializeTransaction()
        {
            var vectorId = "01";
            var vectorReference = "reference.draft";
            var outputFileName = "tx.draft";

            // we might define a TestVector class that holds these values
            // and can be deserialized from dat/{vectorId}/vector.json
            uint fee = 0;
            uint amount = 0;
            var type = "TxBodyMary";
            var description = "";
            var utxo = "98035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d120".HexToByteArray();
            var payment1Addr = "addr_test1vrgvgwfx4xyu3r2sf8nphh4l92y84jsslg5yhyr8xul29rczf3alu".ToAddress();
            var payment2Addr = "addr_test1vqah2xrfp8qjp2tldu8wdq38q8c8tegnduae5zrqff3aeec7g467q".ToAddress();
            byte[] expectedCBOR = "83a3008182582098035740ab68cad12cb4d8281d10ce1112ef0933dc84920b8937c3e80d78d12000018282581d60d0c43926a989c88d5049e61bdebf2a887aca10fa284b9067373ea28f0082581d603b75186909c120a97f6f0ee6822701f075e5136f3b9a08604a63dce70002009ffff6".HexToByteArray();

            // Arrange

            var tx = new TransactionBuilder()
                .WithTransactionBody(new TransactionBodyBuilder()
                    .WithTransactionInputs(new List<TransactionInput>()
                    {
                        new TransactionInputBuilder()
                            .WithTransactionIndex(0)
                            .WithTransactionId(utxo)
                            .Build()
                    })
                    .WithTransactionOutputs(new List<TransactionOutput>()
                    {
                        new TransactionOutputBuilder()
                            .WithAddress(payment1Addr.GetBytes())
                            .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                                .WithCoin(amount)
                                .Build())
                            .Build(),
                        new TransactionOutputBuilder()
                            .WithAddress(payment2Addr.GetBytes())
                            .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                                .WithCoin(amount)
                                .Build())
                            .Build()
                    })
                    .WithFee(fee)
                    .Build())
                .Build();

            // Act
            var cbor = tx.Serialize();
            var cborHex = cbor.ToStringHex();
            var draftTx = new { type, description, cborHex };
            var json = JsonSerializer.Serialize(draftTx, __jsonSerializerOptions);

            WriteVectorFile(vectorId, json, outputFileName);
            string referenceTx = ReadVectorFile(vectorId, vectorReference);

            // Assert
            // ok when i use http://cbor.me/ + https://text-compare.com/ = identical... 
            // unsure exactly whats going on
            // the serialization string/bytes are off a little but i think its a newline or something
            Assert.Equal(referenceTx, json);
            Assert.Equal(expectedCBOR, cbor);

            var expected = _transactionSerializer.DeserializeTransaction(expectedCBOR);
            var deserialized = _transactionSerializer.DeserializeTransaction(cbor);
            Assert.Equal(expected, deserialized);
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

            var baseAddr = _addressService.GetAddress(paymentPub, stakePub, NetworkType.Testnet, AddressType.Base);
            var changeAddr = _addressService.GetAddress(changePub, stakePub, NetworkType.Testnet, AddressType.Base);

            var transactionBody = new TransactionBodyBuilder()
                .WithTransactionInputs(new List<TransactionInput>()
                {
                    new TransactionInputBuilder()
                        .WithTransactionIndex(0)
                        .WithTransactionId(new byte[32])
                        .Build()
                })
                .WithTransactionOutputs(new List<TransactionOutput>()
                {
                    new TransactionOutputBuilder()
                        .WithAddress(baseAddr.GetBytes())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(10)
                            .Build())
                        .Build(),
                    new TransactionOutputBuilder()
                        .WithAddress(changeAddr.GetBytes())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(856488)
                            .Build())
                        .Build()
                })
                .WithTtl(1000)
                .WithFee(143502)
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

            var baseAddr = _addressService.GetAddress(paymentPub, stakePub, NetworkType.Testnet, AddressType.Base);
            var changeAddr = _addressService.GetAddress(changePub, stakePub, NetworkType.Testnet, AddressType.Base);

            var transactionBody = new TransactionBodyBuilder()
                .WithTransactionInputs(new List<TransactionInput>()
                {
                    new TransactionInputBuilder()
                        .WithTransactionIndex(0)
                        .WithTransactionId(new byte[32])
                        .Build()
                })
                .WithTransactionOutputs(new List<TransactionOutput>()
                {
                    new TransactionOutputBuilder()
                        .WithAddress(baseAddr.GetBytes())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(10)
                            .Build())
                        .Build(),
                    new TransactionOutputBuilder()
                        .WithAddress(changeAddr.GetBytes())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(856488)
                            .Build())
                        .Build()
                })
                .WithTtl(1000)
                .WithFee(143502)
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

            var baseAddr = _addressService.GetAddress(paymentPub, stakePub, NetworkType.Testnet, AddressType.Base);

            var transactionBody = new TransactionBodyBuilder()
                .WithTransactionInputs(new List<TransactionInput>()
                {
                    new TransactionInputBuilder()
                        .WithTransactionIndex(0)
                        .WithTransactionId("3b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b7".HexToByteArray())
                        .Build()
                })
                .WithTransactionOutputs(new List<TransactionOutput>()
                {
                    new TransactionOutputBuilder()
                        .WithAddress(baseAddr.GetBytes())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(1)
                            .Build())
                        .Build()
                })
                .WithTtl(10)
                .WithFee(0)
                .Build();

            var witnesses = new TransactionWitnessSetBuilder()
                .WithVKeyWitnesses(new List<VKeyWitness>()
                {
                    new VKeyWitnessBuilder()
                        .WithVKey(paymentPub.Key)
                        .WithSKey(paymentPrv.Key)
                        .Build()
                })
                .Build();

            var transaction = new TransactionBuilder()
                .WithTransactionBody(transactionBody)
                .WithTransactionWitnessSet(witnesses)
                .Build();

            //act
            var serializedTx = transaction.Serialize();

            //assert
            Assert.Equal("83a400818258203b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b70001818258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94010200030aa10081825820489ef28ea97f719ee7768645fc74b811c271e5d7ef06c2310854db30158e945d5840e6489d8cdc11ac139158b878251819a31f01644310fa4a4b9c72c2319aa8887f4e299054346c2ad08016e4b8f55684ccae8bddcc5e2137af730acbed5642ff09f6",
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

            var changeAddr = _addressService.GetAddress(changePub, stakePub, NetworkType.Testnet, AddressType.Base);
            var stakeHash = HashUtility.Blake2b244(stakePub.Key);

            var transactionBody = new TransactionBodyBuilder()
                .WithTransactionInputs(new List<TransactionInput>()
                {
                    new TransactionInputBuilder()
                        .WithTransactionIndex(0)
                        .WithTransactionId(getGenesisTransaction())
                        .Build()
                })
                .WithTransactionOutputs(new List<TransactionOutput>()
                {
                    new TransactionOutputBuilder()
                        .WithAddress(changeAddr.GetBytes())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(3786498)
                            .Build())
                        .Build()
                })
                .WithCertificate(new CertificateBuilder()
                    .WithStakeRegistration(stakeHash)
                    .WithStakeDelegation(new StakeDelegationBuilder()
                        .WithPoolHash(stakeHash)
                        .WithStakeCredential(stakeHash)
                        .Build())
                    .Build())
                .WithTtl(1000)
                .WithFee(213502)
                .Build();

            //act
            var serialized = transactionBody.Serialize(null);

            //assert
            Assert.Equal("a50081825820000000000000000000000000000000000000000000000000000000000000000000018182583900c05e80bdcf267e7fe7bf4a867afe54a65a3605b32aae830ed07f8e1ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d941a0039c702021a000341fe031903e8048282008200581ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d9483028200581ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94581ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d94",
                serialized.ToStringHex());
        }

        [Fact]
        public void MultiAssetTest()
        {
            var rootKey = getBase15WordWallet();

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get change keys
            (var changePrv, var changePub) = getKeyPairFromPath("m/1852'/1815'/0'/1/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            var baseAddr = _addressService.GetAddress(paymentPub, stakePub, NetworkType.Testnet, AddressType.Base);
            var changeAddr = _addressService.GetAddress(changePub, stakePub, NetworkType.Testnet, AddressType.Base);

            var transactionBody = new TransactionBodyBuilder()
                .WithTransactionInputs(new List<TransactionInput>()
                {
                    new TransactionInputBuilder()
                        .WithTransactionIndex(0)
                        .WithTransactionId(getGenesisTransaction())
                        .Build(),
                    new TransactionInputBuilder()
                        .WithTransactionIndex(0)
                        .WithTransactionId(getGenesisTransaction())
                        .Build()
                })
                .WithTransactionOutputs(new List<TransactionOutput>()
                {
                    new TransactionOutputBuilder()
                        .WithAddress(baseAddr.GetBytes())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(1)
                            .WithMultiAsset(new Dictionary<byte[], NativeAsset>() {
                                {
                                    getGenesisPolicyId(),
                                    new NativeAssetBuilder()
                                        .WithToken(new Dictionary<byte[], uint>()
                                        {
                                            { "00010203".HexToByteArray(), 60 }
                                        })
                                        .Build()
                                }
                            })
                            .Build())
                        .Build(),
                    new TransactionOutputBuilder()
                        .WithAddress(changeAddr.GetBytes())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(18)
                            .WithMultiAsset(new Dictionary<byte[], NativeAsset>() {
                                {
                                    getGenesisPolicyId(),
                                    new NativeAssetBuilder()
                                        .WithToken(new Dictionary<byte[], uint>()
                                        {
                                            { "00010203".HexToByteArray(), 240 }
                                        })
                                        .Build()
                                }
                            })
                            .Build())
                        .Build()
                })
                .WithFee(1)
                .Build();

            //act
            var serialized = transactionBody.Serialize(null);

            //assert
            Assert.Equal("a3008282582000000000000000000000000000000000000000000000000000000000000000000082582000000000000000000000000000000000000000000000000000000000000000000001828258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d948201a1581c00000000000000000000000000000000000000000000000000000000a14400010203183c82583900c05e80bdcf267e7fe7bf4a867afe54a65a3605b32aae830ed07f8e1ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d948212a1581c00000000000000000000000000000000000000000000000000000000a1440001020318f00201",
                serialized.ToStringHex());
        }
        
        [Fact]
        public void SimpleTransactionTest()
        {
            //arrange
            var transactionBody = new TransactionBodyBuilder()
                .WithTransactionInputs(new List<TransactionInput>()
                {
                    new TransactionInputBuilder()
                        .WithTransactionIndex(0)
                        .WithTransactionId("3b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b7".HexToByteArray())
                        .Build()
                })
                .WithTransactionOutputs(new List<TransactionOutput>()
                {
                    new TransactionOutputBuilder()
                        .WithAddress("611c616f1acb460668a9b2f123c80372c2adad3583b9c6cd2b1deeed1c".HexToByteArray())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(1)
                            .Build())
                        .Build()
                })
                .WithTtl(10)
                .WithFee(94002)
                .Build();

            var witnesses = new TransactionWitnessSetBuilder()
                .WithVKeyWitnesses(new List<VKeyWitness>()
                {
                    new VKeyWitnessBuilder()
                        .WithVKey("f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee".HexToByteArray())
                        .WithSKey("c660e50315d76a53d80732efda7630cae8885dfb85c46378684b3c6103e1284a".HexToByteArray())
                        .Build()
                })
                .Build();

            var transaction = new TransactionBuilder()
                .WithTransactionBody(transactionBody)
                .WithTransactionWitnessSet(witnesses)
                .Build();

            //act
            var serialized = transaction.Serialize();
            var fee = transaction.CalculateFee();

            //assert
            Assert.Equal("83a400818258203b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b700018182581d611c616f1acb460668a9b2f123c80372c2adad3583b9c6cd2b1deeed1c01021a00016f32030aa10081825820f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee5840fae5de40c94d759ce13bf9886262159c4f26a289fd192e165995b785259e503f6887bf39dfa23a47cf163784c6eee23f61440e749bc1df3c73975f5231aeda0ff6",
                serialized.ToStringHex());
            Assert.Equal(188002, fee);
        }

        [Fact]
        public void MetadataTest()
        {
            //arrange
            var transactionBody = new TransactionBodyBuilder()
                .WithTransactionInputs(new List<TransactionInput>()
                {
                    new TransactionInputBuilder()
                        .WithTransactionIndex(0)
                        .WithTransactionId(getGenesisTransaction())
                        .Build()
                })
                .WithTransactionOutputs(new List<TransactionOutput>()
                {
                    new TransactionOutputBuilder()
                        .WithAddress("00477367D9134E384A25EDD3E23C72735EE6DE6490D39C537A247E1B65D9E5A6498B927F664A2C82343AA6A50CDDE47DE0A2B8C54ECD9C99C2".HexToByteArray())
                        .WithTransactionOutputValue(new TransactionOutputValueBuilder()
                            .WithCoin(1000000)
                            .Build())
                        .Build()
                })
                .WithTtl(10)
                .WithFee(0)
                .Build();

            var witnesses = new TransactionWitnessSetBuilder()
                .WithVKeyWitnesses(new List<VKeyWitness>()
                {
                    new VKeyWitnessBuilder()
                        .WithVKey("0f8ad2c7def332bca2f897ef2a1608ee655341227efe7d2284eeb3f94d08d5fa".HexToByteArray())
                        .WithSKey("501181718c28e401cb77bb31e65e16c125960d225dc615a20d18cce9cd852f4e9af87333cefe80a142e5f270e03d737f6cb3e5e0f27c023d0c4a6380de0a039d".HexToByteArray())
                        .Build()
                })
                .Build();

            var auxData = new AuxiliaryDataBuilder()
                .WithMetadata(new Dictionary<int, object>()
                {
                    { 1234, new { name = "simple message" } }
                })
                .Build();

            var transaction = new TransactionBuilder()
                .WithTransactionBody(transactionBody)
                .WithTransactionWitnessSet(witnesses)
                .WithAuxiliaryData(auxData)
                .Build();

            //act
            var serialized = transaction.Serialize();

            //assert
            Assert.Equal("83a50081825820000000000000000000000000000000000000000000000000000000000000000000018182583900477367d9134e384a25edd3e23c72735ee6de6490d39c537a247e1b65d9e5a6498b927f664a2c82343aa6a50cdde47de0a2b8c54ecd9c99c21a000f42400200030a0758208dc8a798a1da0e2a6df17e66b10a49b5047133dd4daae2686ef1f73369d3fa16a100818258200f8ad2c7def332bca2f897ef2a1608ee655341227efe7d2284eeb3f94d08d5fa584074a7a181addbda26d7974119ac6e3fe35286fb4a6f7a9db573a5e5836808613097256fa2f0284e255cadc566cef96bde750a3ca5cb79a0726349d3424148e00082a11904d2a1646e616d656e73696d706c65206d65737361676580",
                serialized.ToStringHex());
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
