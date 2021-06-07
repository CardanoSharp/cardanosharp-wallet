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
    //Transaction Test Vectors for Cardano Shelley 
    //https://gist.github.com/KtorZ/5a2089df0915f21aca368d12545ab230
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

        [Fact]
        public void SerializeBodyTest()
        {
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
                        Address = "0079467C69A9AC66280174D09D62575BA955748B21DEC3B483A9469A65CC339A35F9E0FE039CF510C761D4DD29040C48E9657FDAC7E9C01D94".HexToByteArray(),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 10
                        }
                    },
                    new TransactionOutput()
                    {
                        Address = "00C05E80BDCF267E7FE7BF4A867AFE54A65A3605B32AAE830ED07F8E1CCC339A35F9E0FE039CF510C761D4DD29040C48E9657FDAC7E9C01D94".HexToByteArray(),
                        Value = new TransactionOutputValue()
                        {
                            Coin = 856488
                        }
                    }
                },
                Fee = 143502,
                Ttl = 1000
            };

            var serialized = _transactionBuilder.SerializeBody(transactionBody);

            Assert.Equal("a4008182582000000000000000000000000000000000000000000000000000000000000000000001828258390079467c69a9ac66280174d09d62575ba955748b21dec3b483a9469a65cc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d940a82583900c05e80bdcf267e7fe7bf4a867afe54a65a3605b32aae830ed07f8e1ccc339a35f9e0fe039cf510c761d4dd29040c48e9657fdac7e9c01d941a000d11a8021a0002308e031903e8",
                serialized.ToStringHex());
        }


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

            var transaction = new Transaction()
            {
                TransactionBody = new TransactionBody()
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
                        }
                    },
                    Ttl = 1000,
                    Fee = 0
                },
                TransactionWitnessSet = new TransactionWitnessSet()
                {
                    VKeyWitnesses = new List<VKeyWitness>() 
                    { 
                        new VKeyWitness()
                        {
                            VKey = stakePub,
                            Signature = stakePrv.Item1
                        }
                    }
                }
            };

            //act

            //assert
        }

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
