using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Common;
using System.IO;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using CardanoSharp.Wallet.Extensions.Models;

namespace CardanoSharp.Wallet.Test
{
    public class TransactionTests
    {
        private readonly TransactionBuilder _transactionBuilder;
        private readonly IKeyService _keyService;
        private readonly IAddressService _addressService;
        public TransactionTests()
        {
            _keyService = new KeyService();
            _addressService = new AddressService();
            _transactionBuilder = new TransactionBuilder();
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

            var transactionBody = new TransactionBody()
            {
                TransactionInputs = new List<TransactionInput>()
                {
                    new TransactionInput()
                    {
                        TransactionIndex = 0,
                        TransactionId = getGenesisTransaction()
                    }
                },
                TransactionOutputs = new List<TransactionOutput>()
                {
                    new TransactionOutput()
                    {
                        Address = _addressService.GetAddressBytes(baseAddr),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 10
                        }
                    },
                    new TransactionOutput()
                    {
                        Address = _addressService.GetAddressBytes(changeAddr),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 856488
                        }
                    }
                },
                Ttl = 1000,
                Fee = 143502
            };

            //act
            var serialized = _transactionBuilder.SerializeBody(transactionBody);

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
            var changeAddr = _addressService.GetAddress(changePub, stakePub, NetworkType.Testnet, AddressType.Base);

            var transactionBody = new TransactionBody()
            {
                TransactionInputs = new List<TransactionInput>()
                {
                    new TransactionInput()
                    {
                        TransactionIndex = 0,
                        TransactionId = "3b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b7".HexToByteArray()
                    }
                },
                TransactionOutputs = new List<TransactionOutput>()
                {
                    new TransactionOutput()
                    {
                        Address = _addressService.GetAddressBytes(baseAddr),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 1
                        }
                    }
                },
                Ttl = 10,
                Fee = 0
            };

            var witnesses = new TransactionWitnessSet()
            {
                VKeyWitnesses = new List<VKeyWitness>()
                {
                    new VKeyWitness()
                    {
                        VKey = paymentPub,
                        SKey = paymentPrv
                    }
                }
            };

            var transaction = new Transaction()
            {
                TransactionBody = transactionBody,
                TransactionWitnessSet = witnesses
            };

            //act
            var serializedTx = _transactionBuilder.SerializeTransaction(transaction);

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
            var stakeHash = HashHelper.Blake2b244(stakePub);

            var transactionBody = new TransactionBody()
            {
                TransactionInputs = new List<TransactionInput>()
                {
                    new TransactionInput()
                    {
                        TransactionIndex = 0,
                        TransactionId = getGenesisTransaction()
                    }
                },
                TransactionOutputs = new List<TransactionOutput>()
                {
                    new TransactionOutput()
                    {
                        Address = _addressService.GetAddressBytes(changeAddr),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 3786498
                        }
                    }
                },
                Ttl = 1000,
                Fee = 213502,
                Certificate = new Certificate()
                {
                    StakeRegistration = stakeHash,
                    StakeDelegation = new StakeDelegation()
                    {
                        PoolHash = stakeHash,
                        StakeCredential = stakeHash
                    }
                }
            };

            //act
            var serialized = _transactionBuilder.SerializeBody(transactionBody);

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

            var transactionBody = new TransactionBody()
            {
                TransactionInputs = new List<TransactionInput>()
                {
                    new TransactionInput()
                    {
                        TransactionIndex = 0,
                        TransactionId = getGenesisTransaction()
                    },
                    new TransactionInput()
                    {
                        TransactionIndex = 0,
                        TransactionId = getGenesisTransaction()
                    }
                },
                TransactionOutputs = new List<TransactionOutput>()
                {
                    new TransactionOutput()
                    {
                        Address = _addressService.GetAddressBytes(baseAddr),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 1,
                            MultiAsset = new Dictionary<byte[], NativeAsset>()
                            {
                                {
                                    getGenesisPolicyId(),
                                    new NativeAsset()
                                    {
                                        Token = new Dictionary<AssetName, uint>()
                                        {
                                            { new AssetName() { BytesValue = "00010203".HexToByteArray() }, 60 }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new TransactionOutput()
                    {
                        Address = _addressService.GetAddressBytes(changeAddr),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 18,
                            MultiAsset = new Dictionary<byte[], NativeAsset>()
                            {
                                {
                                    getGenesisPolicyId(),
                                    new NativeAsset()
                                    {
                                        Token = new Dictionary<AssetName, uint>()
                                        {
                                            { new AssetName() { BytesValue = "00010203".HexToByteArray() }, 240 }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Fee = 1
            };

            //act
            var serialized = _transactionBuilder.SerializeBody(transactionBody);

            //assert
            Assert.Equal("a3008282582000000000000000000000000000000000000000000000000000000000000000000082582000000000000000000000000000000000000000000000000000000000000000000001828258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d948201a1581c00000000000000000000000000000000000000000000000000000000a14400010203183c82583900c05e80bdcf267e7fe7bf4a867afe54a65a3605b32aae830ed07f8e1ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d948212a1581c00000000000000000000000000000000000000000000000000000000a1440001020318f00201",
                serialized.ToStringHex());
        }
        
        [Fact]
        public void SimpleTransactionTest()
        {
            //arrange
            var transactionBody = new TransactionBody()
            {
                TransactionInputs = new List<TransactionInput>()
                {
                    new TransactionInput()
                    {
                        TransactionIndex = 0,
                        TransactionId = "3b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b7".HexToByteArray()
                    }
                },
                TransactionOutputs = new List<TransactionOutput>()
                {
                    new TransactionOutput()
                    {
                        Address = "611c616f1acb460668a9b2f123c80372c2adad3583b9c6cd2b1deeed1c".HexToByteArray(),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 1
                        }
                    }
                },
                Ttl = 10,
                Fee = 94002
            };

            var witnesses = new TransactionWitnessSet()
            {
                VKeyWitnesses = new List<VKeyWitness>()
                {
                    new VKeyWitness()
                    {
                        VKey = "f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee".HexToByteArray(),
                        SKey = "c660e50315d76a53d80732efda7630cae8885dfb85c46378684b3c6103e1284a".HexToByteArray()
                    }
                }
            };

            var transaction = new Transaction()
            {
                TransactionBody = transactionBody,
                TransactionWitnessSet = witnesses
            };

            //act
            var serialized = _transactionBuilder.SerializeTransaction(transaction);
            var fee = _transactionBuilder.CalculateFee(serialized);

            //assert
            Assert.Equal("83a400818258203b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b700018182581d611c616f1acb460668a9b2f123c80372c2adad3583b9c6cd2b1deeed1c01021a00016f32030aa10081825820f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee5840fae5de40c94d759ce13bf9886262159c4f26a289fd192e165995b785259e503f6887bf39dfa23a47cf163784c6eee23f61440e749bc1df3c73975f5231aeda0ff6",
                serialized.ToStringHex());
            Assert.Equal(94002, fee);
        }

        [Fact]
        public void MetadataTest()
        {
            //arrange
            var transactionBody = new TransactionBody()
            {
                TransactionInputs = new List<TransactionInput>()
                {
                    new TransactionInput()
                    {
                        TransactionIndex = 0,
                        TransactionId = "0000000000000000000000000000000000000000000000000000000000000000".HexToByteArray()
                    }
                },
                TransactionOutputs = new List<TransactionOutput>()
                {
                    new TransactionOutput()
                    {
                        Address = "00477367D9134E384A25EDD3E23C72735EE6DE6490D39C537A247E1B65D9E5A6498B927F664A2C82343AA6A50CDDE47DE0A2B8C54ECD9C99C2".HexToByteArray(),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 1000000
                        }
                    }
                },
                Ttl = 10,
                Fee = 0
            };

            var auxData = new AuxiliaryData()
            {
                Metadata = new Dictionary<int, object>()
                {
                    { 1234, new { name = "simple message" } }
                }
            };

            var witnesses = new TransactionWitnessSet()
            {
                VKeyWitnesses = new List<VKeyWitness>()
                {
                    new VKeyWitness()
                    {
                        VKey = "0f8ad2c7def332bca2f897ef2a1608ee655341227efe7d2284eeb3f94d08d5fa".HexToByteArray(),
                        SKey = "501181718c28e401cb77bb31e65e16c125960d225dc615a20d18cce9cd852f4e9af87333cefe80a142e5f270e03d737f6cb3e5e0f27c023d0c4a6380de0a039d".HexToByteArray()
                    }
                }
            };

            var transaction = new Transaction()
            {
                TransactionBody = transactionBody,
                TransactionWitnessSet = witnesses,
                AuxiliaryData = auxData
            };

            //act
            var serialized = _transactionBuilder.SerializeTransaction(transaction);

            //assert
            Assert.Equal("83a50081825820000000000000000000000000000000000000000000000000000000000000000000018182583900477367d9134e384a25edd3e23c72735ee6de6490d39c537a247e1b65d9e5a6498b927f664a2c82343aa6a50cdde47de0a2b8c54ecd9c99c21a000f42400200030a0758208dc8a798a1da0e2a6df17e66b10a49b5047133dd4daae2686ef1f73369d3fa16a100818258200f8ad2c7def332bca2f897ef2a1608ee655341227efe7d2284eeb3f94d08d5fa584074a7a181addbda26d7974119ac6e3fe35286fb4a6f7a9db573a5e5836808613097256fa2f0284e255cadc566cef96bde750a3ca5cb79a0726349d3424148e00082a11904d2a1646e616d656e73696d706c65206d65737361676580",
                serialized.ToStringHex());
        }

        [Fact]
        public void MintingTest()
        {
            var mnemonic = "object dwarf meadow dry figure return february once become eye cricket circle repair security palm year secret wine blind phone brown rain tissue spread";
            var entropy = _keyService.Restore(mnemonic);
            var rootKey = _keyService.GetRootKey(entropy);

            //get payment keys
            (var paymentPrv, var paymentPub) = getKeyPairFromPath("m/1852'/1815'/0'/0/0", rootKey);

            //get stake keys
            (var stakePrv, var stakePub) = getKeyPairFromPath("m/1852'/1815'/0'/2/0", rootKey);

            //get delegation address
            var baseAddr = _addressService.GetAddress(paymentPub, stakePub, NetworkType.Testnet, AddressType.Base);

            //policy info
            var policySkey = "a1fef97babefc02bb927cb56c19308503e297607b1dbdfc72941ebdd388ade6f".HexToByteArray();
            var policyVkey = "848AC717B552FCD1F2DCB4933E4A8198187E7E424693B51E1B8B16250F3CADFE".HexToByteArray();
            var policyKeyHash = HashHelper.Blake2b244(policyVkey);
            var policyScript = new ScriptAll()
            {
                NativeScripts = new List<NativeScript>()
                {
                    new NativeScript()
                    {
                        ScriptPubKey = new ScriptPubKey()
                        {
                            KeyHash = policyKeyHash
                        }
                    } 
                }
            };
            var policyId = policyScript.GetPolicyId();

            uint txInIndex = 0;
            string txInAddr = "b7f62d53d30d785f5a72d6b75c31214e721886224fdedbb70c2b4932bb156d5a";

            AssetName assetName = new AssetName()
            {
                StringValue = "sharptest"
            };
            uint assetAmount = 1;

            var transactionBody = new TransactionBody()
            {
                TransactionInputs = new List<TransactionInput>()
                {
                    new TransactionInput()
                    {
                        TransactionIndex = txInIndex,
                        TransactionId = txInAddr.HexToByteArray()
                    }
                },
                TransactionOutputs = new List<TransactionOutput>()
                {
                    new TransactionOutput()
                    {
                        Address = _addressService.GetAddressBytes(baseAddr),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 999814875,
                            MultiAsset = new Dictionary<byte[], NativeAsset>()
                            {
                                {
                                    policyId,
                                    new NativeAsset()
                                    {
                                        Token = new Dictionary<AssetName, uint>()
                                        {
                                            { assetName, assetAmount }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Fee = 185125,
                Mint = new Dictionary<byte[], NativeAsset>()
                {
                    {
                        policyId,
                        new NativeAsset()
                        {
                            Token = new Dictionary<AssetName, uint>()
                            {
                                { assetName, assetAmount }
                            }
                        }
                    }
                }
            };

            var witnesses = new TransactionWitnessSet()
            {
                VKeyWitnesses = new List<VKeyWitness>()
                {
                    new VKeyWitness()
                    {
                        VKey = paymentPub,
                        SKey = paymentPrv
                    },
                    new VKeyWitness()
                    {
                        VKey = policyVkey,
                        SKey = policySkey
                    }
                },
                NativeScripts = new List<NativeScript>() 
                { 
                    new NativeScript()
                    {
                        ScriptAll = policyScript
                    }
                }
            };

            var auxData = new AuxiliaryData()
            {
                Metadata = new Dictionary<int, object>()
                {
                    { 1337, new { message = "sharptest" } }
                }
            };

            var transaction = new Transaction()
            {
                TransactionBody = transactionBody,
                TransactionWitnessSet = witnesses,
                AuxiliaryData = auxData
            };

            var signedTx = _transactionBuilder.SerializeTransaction(transaction);
            var fee = _transactionBuilder.CalculateFee(signedTx, 44, 155381);
            //transactionBody.Fee = (uint)((fee < 155381) ? 155381 : fee);
            signedTx = _transactionBuilder.SerializeTransaction(transaction);
            var signedTxStr = signedTx.ToStringHex();
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

        private (byte[], byte[]) getBase15WordWallet()
        {
            var mnemonic = "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy";
            var entropy = _keyService.Restore(mnemonic);
            return _keyService.GetRootKey(entropy);
        }

        private (byte[], byte[]) getKeyPairFromPath(string path, (byte[], byte[]) rootKey)
        {
            var privateKey = _keyService.DerivePath(path, rootKey.Item1, rootKey.Item2);
            var publicKey = _keyService.GetPublicKey(privateKey.Item1, false);
            return (privateKey.Item1, publicKey);
        }
    }
}
