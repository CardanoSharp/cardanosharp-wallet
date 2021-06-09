using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CardanoSharp.Wallet.Extensions;

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
            var mnemonic = "art forum devote street sure rather head chuckle guard poverty release quote oak craft enemy";
            var entropy = _keyService.Restore(mnemonic);
            var rootKey = _keyService.GetRootKey(entropy);

            //get payment keys
            var paymentPath = "m/1852'/1815'/0'/0/0";
            var paymentPrv = _keyService.DerivePath(paymentPath, rootKey.Item1, rootKey.Item2);
            var paymentPub = _keyService.GetPublicKey(paymentPrv.Item1, false);

            //get change keys
            var changePath = "m/1852'/1815'/0'/1/0";
            var changePrv = _keyService.DerivePath(changePath, rootKey.Item1, rootKey.Item2);
            var changePub = _keyService.GetPublicKey(changePrv.Item1, false);

            //get stake keys
            var stakePath = "m/1852'/1815'/0'/2/0";
            var stakePrv = _keyService.DerivePath(stakePath, rootKey.Item1, rootKey.Item2);
            var stakePub = _keyService.GetPublicKey(stakePrv.Item1, false);

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

        #region IOHK Transaction Test Vectors for Cardano Shelley
        //https://gist.github.com/KtorZ/5a2089df0915f21aca368d12545ab230

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

            //assert
            Assert.Equal("83a400818258203b40265111d8bb3c3c608d95b3a0bf83461ace32d79336579a1939b3aad1c0b700018182581d611c616f1acb460668a9b2f123c80372c2adad3583b9c6cd2b1deeed1c01021a00016f32030aa10081825820f9aa3fccb7fe539e471188ccc9ee65514c5961c070b06ca185962484a4813bee5840fae5de40c94d759ce13bf9886262159c4f26a289fd192e165995b785259e503f6887bf39dfa23a47cf163784c6eee23f61440e749bc1df3c73975f5231aeda0ff6",
                serialized.ToStringHex());
        }
        #endregion

        private byte[] getGenesisTransaction()
        {
            var hash = new byte[32];
            for(var i = 0; i < hash.Length; i++)
            {
                hash[i] = 0x00;
            }
            return hash;
        }
    }
}
