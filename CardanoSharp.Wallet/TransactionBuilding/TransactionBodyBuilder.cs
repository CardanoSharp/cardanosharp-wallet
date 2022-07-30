using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionBodyBuilder: IABuilder<TransactionBody>
    {
        ITransactionBodyBuilder AddInput(byte[] transactionId, uint transactionIndex);
        ITransactionBodyBuilder AddInput(string transactionId, uint transactionIndex);
        ITransactionBodyBuilder AddOutput(byte[] address, ulong coin, ITokenBundleBuilder tokenBundleBuilder = null);
        ITransactionBodyBuilder AddOutput(Address address, ulong coin, ITokenBundleBuilder tokenBundleBuilder = null);
        ITransactionBodyBuilder SetCertificate(ICertificateBuilder certificateBuilder);
        ITransactionBodyBuilder SetFee(ulong fee);
        ITransactionBodyBuilder SetTtl(uint ttl);
        ITransactionBodyBuilder SetMint(ITokenBundleBuilder token = null, ITokenBurnBuilder burnToken = null);
    }

    public class TransactionBodyBuilder : ABuilder<TransactionBody>, ITransactionBodyBuilder
    {
        private TransactionBodyBuilder()
        {
            _model = new TransactionBody();
        }

        private TransactionBodyBuilder(TransactionBody model)
        {
            _model = model;
        }

        public static ITransactionBodyBuilder GetBuilder(TransactionBody model)
        {
            if (model == null)
            {
                return new TransactionBodyBuilder();
            }
            return new TransactionBodyBuilder(model);
        }

        public static ITransactionBodyBuilder Create
        {
            get => new TransactionBodyBuilder();
        }

        public ITransactionBodyBuilder SetCertificate(ICertificateBuilder certificateBuilder)
        {
            _model.Certificate = certificateBuilder.Build();
            return this;
        }

        public ITransactionBodyBuilder AddInput(byte[] transactionId, uint transactionIndex)
        {

            _model.TransactionInputs.Add(new TransactionInput()
            {
                TransactionId = transactionId,
                TransactionIndex = transactionIndex
            });
            return this;
        }

        public ITransactionBodyBuilder AddInput(string transactionIdStr, uint transactionIndex)
        {
            byte[] transactionId = transactionIdStr.HexToByteArray();
            _model.TransactionInputs.Add(new TransactionInput()
            {
                TransactionId = transactionId,
                TransactionIndex = transactionIndex
            });
            return this;
        }

        public ITransactionBodyBuilder AddOutput(Address address, ulong coin, ITokenBundleBuilder tokenBundleBuilder = null)
        {
            return AddOutput(address.GetBytes(), coin, tokenBundleBuilder);
        }

        public ITransactionBodyBuilder AddOutput(byte[] address, ulong coin, ITokenBundleBuilder tokenBundleBuilder = null)
        {
            var outputValue = new TransactionOutputValue()
            {
                Coin = coin
            };

            if (tokenBundleBuilder != null)
            {
                outputValue.MultiAsset = tokenBundleBuilder.Build();
            }

            _model.TransactionOutputs.Add(new TransactionOutput()
            {
                Address = address,
                Value = outputValue
            });
            return this;
        }

        public ITransactionBodyBuilder SetFee(ulong fee)
        {
            _model.Fee = fee;
            return this;
        }

        public ITransactionBodyBuilder SetTtl(uint ttl)
        {
            _model.Ttl = ttl;
            return this;
        }

        public ITransactionBodyBuilder SetMint(ITokenBundleBuilder tokenBundleBuilder = null, ITokenBurnBuilder tokenBurnBuilder = null)
        {
            Dictionary<byte[], NativeAsset<long>> mint = new Dictionary<byte[], NativeAsset<long>>();

            if (tokenBurnBuilder != null) {
                var tokenBurn = tokenBurnBuilder.Build();
                foreach (var tokenBurnPair in tokenBurn) {
                    var policyId = tokenBurnPair.Key;
                    var asset = tokenBurnPair.Value;
                    bool isPolicyInMint = mint.TryGetValue(policyId, out NativeAsset<long> nativeAsset);
                    if (!isPolicyInMint) {
                        nativeAsset = new NativeAsset<long>();
                        mint.Add(policyId, nativeAsset);
                    }
                    
                    foreach (var tokenPair in asset.Token) {
                        var assetId = tokenPair.Key;
                        var amount = tokenPair.Value;
                        nativeAsset.Token.Add(tokenPair.Key, Convert.ToInt64(tokenPair.Value));
                    }        
                }
            }
            
            if (tokenBundleBuilder != null) {
                var tokenBundle = tokenBundleBuilder.Build();
                foreach (var tokenBundlePair in tokenBundle) {
                    var policyId = tokenBundlePair.Key;
                    var asset = tokenBundlePair.Value;
                    bool isPolicyInMint = mint.TryGetValue(policyId, out NativeAsset<long> nativeAsset);
                    if (!isPolicyInMint) {
                        nativeAsset = new NativeAsset<long>();
                        mint.Add(policyId, nativeAsset);
                    }
                    
                    foreach (var tokenPair in asset.Token) {
                        var assetId = tokenPair.Key;
                        var amount = tokenPair.Value;
                        nativeAsset.Token.Add(tokenPair.Key, Convert.ToInt64(tokenPair.Value));
                    }        
                }
            }

            _model.Mint = mint; 
            return this;
        }
    }
}
