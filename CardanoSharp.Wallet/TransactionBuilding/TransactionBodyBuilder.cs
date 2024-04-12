﻿using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using CardanoSharp.Wallet.Utilities;


namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITransactionBodyBuilder: IABuilder<TransactionBody>
    {
        ITransactionBodyBuilder AddInput(TransactionInput transactionInput);
        ITransactionBodyBuilder AddInput(string transactionId, uint transactionIndex);
        ITransactionBodyBuilder AddInput(byte[] transactionId, uint transactionIndex);
        ITransactionBodyBuilder AddOutput(TransactionOutput transactionOutput);
        ITransactionBodyBuilder AddOutput(Address address, ulong coin, ITokenBundleBuilder tokenBundleBuilder = null, 
                DatumOption? datumOption = null, 
                ScriptReference? scriptReference = null, 
                OutputPurpose outputPurpose = OutputPurpose.Spend);
        ITransactionBodyBuilder AddOutput(byte[] address, ulong coin, ITokenBundleBuilder tokenBundleBuilder = null,
            DatumOption? datumOption = null, 
            ScriptReference? scriptReference = null, 
            OutputPurpose outputPurpose = OutputPurpose.Spend);
        ITransactionBodyBuilder SetCertificate(ICertificateBuilder certificateBuilder);
        ITransactionBodyBuilder SetFee(ulong fee);
        ITransactionBodyBuilder SetTtl(uint ttl);
        ITransactionBodyBuilder SetMetadataHash(IAuxiliaryDataBuilder auxiliaryDataBuilder);
        ITransactionBodyBuilder SetMint(ITokenBundleBuilder token);
        ITransactionBodyBuilder SetScriptDataHash(byte[] scriptDataHash);
        ITransactionBodyBuilder SetScriptDataHash(List<Redeemer> redeemers, List<IPlutusData> datums);
        ITransactionBodyBuilder SetScriptDataHash(List<Redeemer> redeemers, List<IPlutusData> datums, byte[] languageViews);
        ITransactionBodyBuilder AddCollateralInput(TransactionInput transactionInput);
        ITransactionBodyBuilder AddCollateralInput(byte[] transactionId, uint transactionIndex);
        ITransactionBodyBuilder AddCollateralInput(string transactionIdStr, uint transactionIndex);
        ITransactionBodyBuilder AddRequiredSigner(byte[] requiredSigner);
        ITransactionBodyBuilder SetNetworkId(uint networkId);
        ITransactionBodyBuilder SetCollateralOutput(TransactionOutput transactionOutput);
        ITransactionBodyBuilder SetCollateralOutput(Address address, ulong coin);
        ITransactionBodyBuilder SetCollateralOutput(byte[] address, ulong coin);
        ITransactionBodyBuilder SetTotalCollateral(ulong TotalCollateral);
        ITransactionBodyBuilder AddReferenceInput(TransactionInput transactionInput);
        ITransactionBodyBuilder AddReferenceInput(byte[] transactionId, uint transactionIndex);
        ITransactionBodyBuilder AddReferenceInput(string transactionIdStr, uint transactionIndex);
        ITransactionBodyBuilder RemoveFeeFromChange(ulong? fee = null);
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

        public ITransactionBodyBuilder AddInput(TransactionInput transactionInput)
        {

            _model.TransactionInputs.Add(transactionInput);
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

        public ITransactionBodyBuilder AddOutput(TransactionOutput transactionOutput)
        {
            _model.TransactionOutputs.Add(transactionOutput);
            return this;
        }

        public ITransactionBodyBuilder AddOutput(Address address, ulong coin, 
            ITokenBundleBuilder? tokenBundleBuilder = null, 
            DatumOption? datumOption = null, 
            ScriptReference? scriptReference = null, OutputPurpose outputPurpose = OutputPurpose.Spend)
        {
            return AddOutput(address.GetBytes(), coin, tokenBundleBuilder, datumOption, scriptReference, outputPurpose);
        }

        public ITransactionBodyBuilder AddOutput(byte[] address, ulong coin, 
            ITokenBundleBuilder? tokenBundleBuilder = null,
            DatumOption? datumOption = null, 
            ScriptReference? scriptReference = null, OutputPurpose outputPurpose = OutputPurpose.Spend)
        {
            var outputValue = new TransactionOutputValue()
            {
                Coin = coin
            };

            if (tokenBundleBuilder != null)
            {
                outputValue.MultiAsset = tokenBundleBuilder.Build();
            }

            var output = new TransactionOutput()
            {
                Address = address,
                Value = outputValue,
                OutputPurpose = outputPurpose
            };
            
            if (datumOption is not null)
                output.DatumOption = datumOption;
        
            if (scriptReference is not null)
                output.ScriptReference = scriptReference;

            _model.TransactionOutputs.Add(output);
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

        public ITransactionBodyBuilder SetCertificate(ICertificateBuilder certificateBuilder)
        {
            _model.Certificate = certificateBuilder.Build();
            return this;
        }

        public ITransactionBodyBuilder SetMetadataHash(IAuxiliaryDataBuilder auxiliaryDataBuilder) {
            _model.MetadataHash = HashUtility.Blake2b256(auxiliaryDataBuilder.Build().GetCBOR().EncodeToBytes()).ToStringHex();
            return this;
        }

        public ITransactionBodyBuilder SetMint(ITokenBundleBuilder tokenBuilder)
        {
            _model.Mint = tokenBuilder.Build();
            return this;
        }

        public ITransactionBodyBuilder SetScriptDataHash(byte[] scriptDataHash) 
        {
            _model.ScriptDataHash = scriptDataHash;
            return this;
        }

        public ITransactionBodyBuilder SetScriptDataHash(List<Redeemer> redeemers, List<IPlutusData> datums) 
        {
            return SetScriptDataHash(redeemers, datums, CostModelUtility.PlutusV2CostModel.Serialize());
        }

        public ITransactionBodyBuilder SetScriptDataHash(List<Redeemer> redeemers, List<IPlutusData> datums, byte[] languageViews) 
        {
            _model.ScriptDataHash = ScriptUtility.GenerateScriptDataHash(redeemers, datums, languageViews);
            return this;
        }

        public ITransactionBodyBuilder AddCollateralInput(TransactionInput transactionInput)
        {
            if (_model.Collateral is null) 
            {
                _model.Collateral = new List<TransactionInput>();
            }

            _model.Collateral.Add(transactionInput);
            return this;
        }

        public ITransactionBodyBuilder AddCollateralInput(byte[] transactionId, uint transactionIndex)
        {
            if (_model.Collateral is null) 
            {
                _model.Collateral = new List<TransactionInput>();
            }

            _model.Collateral.Add(new TransactionInput()
            {
                TransactionId = transactionId,
                TransactionIndex = transactionIndex
            });
            return this;
        }

        public ITransactionBodyBuilder AddCollateralInput(string transactionIdStr, uint transactionIndex)
        {
            if (_model.Collateral is null) 
            {
                _model.Collateral = new List<TransactionInput>();
            }

            byte[] transactionId = transactionIdStr.HexToByteArray();
            _model.Collateral.Add(new TransactionInput()
            {
                TransactionId = transactionId,
                TransactionIndex = transactionIndex
            });
            return this;
        }

        public ITransactionBodyBuilder AddRequiredSigner(byte[] requiredSigner) 
        {
            if (_model.RequiredSigners is null) 
            {
                _model.RequiredSigners = new List<byte[]>();
            }

            _model.RequiredSigners.Add(requiredSigner);
            return this;
        }

        public ITransactionBodyBuilder SetNetworkId(uint networkId) {
            _model.NetworkId = networkId;
            return this;
        }

        public ITransactionBodyBuilder SetCollateralOutput(TransactionOutput transactionOutput)
        {
            _model.CollateralReturn = transactionOutput;
            return this;
        }

        public ITransactionBodyBuilder SetCollateralOutput(Address address, ulong coin)
        {
            return SetCollateralOutput(address.GetBytes(), coin);
        }

        public ITransactionBodyBuilder SetCollateralOutput(byte[] address, ulong coin)
        {
            var outputValue = new TransactionOutputValue()
            {
                Coin = coin
            };

            var output = new TransactionOutput()
            {
                Address = address,
                Value = outputValue,
                OutputPurpose = OutputPurpose.Collateral
            };

            _model.CollateralReturn = output;
            return this;
        }

        public ITransactionBodyBuilder SetTotalCollateral(ulong totalCollateral)
        {
            _model.TotalCollateral = totalCollateral;
            return this;
        }

        public ITransactionBodyBuilder AddReferenceInput(TransactionInput transactionInput)
        {
            if (_model.ReferenceInputs is null)
            {
                _model.ReferenceInputs = new List<TransactionInput>();
            }

            _model.ReferenceInputs.Add(transactionInput);
            return this;
        }

        public ITransactionBodyBuilder AddReferenceInput(byte[] transactionId, uint transactionIndex)
        {
            if (_model.ReferenceInputs is null)
            {
                _model.ReferenceInputs = new List<TransactionInput>();
            }

            _model.ReferenceInputs.Add(new TransactionInput()
            {
                TransactionId = transactionId,
                TransactionIndex = transactionIndex
            });
            return this;
        }

        public ITransactionBodyBuilder AddReferenceInput(string transactionIdStr, uint transactionIndex)
        {
            if (_model.ReferenceInputs is null)
            {
                _model.ReferenceInputs = new List<TransactionInput>();
            }

            byte[] transactionId = transactionIdStr.HexToByteArray();
            _model.ReferenceInputs.Add(new TransactionInput()
            {
                TransactionId = transactionId,
                TransactionIndex = transactionIndex
            });
            return this;
        }

        // Helper Functions
        public ITransactionBodyBuilder RemoveFeeFromChange(ulong? fee = null)
        {
            if (fee is null)
                fee = _model.Fee;
            
            //get count of change outputs to deduct fee from evenly
            //note we are selecting only ones that dont have assets
            //  this is to respect minimum ada required for token bundles
            IEnumerable<TransactionOutput> changeOutputs;
            if(_model.TransactionOutputs
               .Any(x => x.OutputPurpose == OutputPurpose.Change
                           && (x.Value.MultiAsset is null 
                               || (x.Value.MultiAsset is not null 
                                   && !x.Value.MultiAsset.Any()))))
            {
                changeOutputs = _model.TransactionOutputs
                    .Where(x => x.OutputPurpose == OutputPurpose.Change
                                && (x.Value.MultiAsset is null
                                    || (x.Value.MultiAsset is not null
                                        && !x.Value.MultiAsset.Any())));
            }
            else
            {
                changeOutputs = _model.TransactionOutputs
                    .Where(x => x.OutputPurpose == OutputPurpose.Change);
            }
            
            ulong feePerChangeOutput = fee.Value / (ulong)changeOutputs.Count();
            ulong feeRemaining = fee.Value % (ulong)changeOutputs.Count();
            bool needToApplyRemaining = true;
            foreach (var o in changeOutputs)
            {
                if (needToApplyRemaining)
                {
                    o.Value.Coin = o.Value.Coin - feePerChangeOutput - feeRemaining;
                    needToApplyRemaining = false;
                }else 
                    o.Value.Coin = o.Value.Coin - feePerChangeOutput;
            }

            return this;
        }
    }
}
