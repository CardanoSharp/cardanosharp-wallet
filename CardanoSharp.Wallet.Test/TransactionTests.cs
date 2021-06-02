using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
