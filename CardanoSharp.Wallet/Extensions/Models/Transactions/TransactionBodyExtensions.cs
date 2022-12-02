using CardanoSharp.Wallet.Extensions.Models.Certificates;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Utilities;
using PeterO.Cbor2;
using System;
using System.Linq;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static class TransactionBodyExtensions
    {
        public static CBORObject GetCBOR(this TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            CBORObject cborBody = CBORObject.NewMap();

            CBORObject cborInputs = null;
            CBORObject cborOutputs = null;
            CBORObject cborTransactionMint = null;
            CBORObject cborCollateralInputs = null;
            CBORObject cborRequiredSigners = null;
            CBORObject cborReferenceInputs = null;

            //add all the transaction inputs
            if (transactionBody.TransactionInputs.Any())
            {
                cborInputs = CBORObject.NewArray();
                foreach (var txInput in transactionBody.TransactionInputs)
                {
                    cborInputs.Add(txInput.GetCBOR());
                }
            }

            if (cborInputs != null) cborBody.Add(0, cborInputs);


            //add all the transaction outputs
            if (transactionBody.TransactionOutputs.Any())
            {
                cborOutputs = CBORObject.NewArray();
                foreach (var txOutput in transactionBody.TransactionOutputs)
                {
                    cborOutputs.Add(txOutput.GetCBOR());
                }
            }

            if (cborOutputs != null) cborBody.Add(1, cborOutputs);

            //add fee
            cborBody.Add(2, transactionBody.Fee);

            //add ttl
            if (transactionBody.Ttl.HasValue) cborBody.Add(3, transactionBody.Ttl.Value);

            //add certificates
            if (transactionBody.Certificate != null)
            {
                cborBody.Add(4, transactionBody.Certificate.GetCBOR());
            }

            // 5) Withdrawals
            // 6) Update

            // 7) add metadata
            if (auxiliaryData != null || transactionBody.MetadataHash != default)
            {
                if (auxiliaryData != null) {
                    cborBody.Add(7, HashUtility.Blake2b256(auxiliaryData.Serialize()));
                }
                else if (transactionBody.MetadataHash != default) {
                    cborBody.Add(7, transactionBody.MetadataHash.HexToByteArray());
                }
            }

            // 8) validity interval start

            // 9) add tokens for minting
            if(transactionBody.Mint.Any())
            {
                cborTransactionMint = CBORObject.NewMap();
                foreach (var nativeAsset in transactionBody.Mint)
                {
                    var assetCbor = CBORObject.NewMap();
                    foreach (var asset in nativeAsset.Value.Token)
                    {
                        assetCbor.Add(asset.Key, asset.Value);
                    }
                    cborTransactionMint.Add(nativeAsset.Key, assetCbor);
                }
                cborBody.Add(9, cborTransactionMint);
            }

            // 11) script_data_hash
            if (transactionBody.ScriptDataHash != null)
            {
                cborBody.Add(11, transactionBody.ScriptDataHash);
            }

            // 13) collateral_inputs
            if (transactionBody.Collateral.Any())
            {
                cborCollateralInputs = CBORObject.NewArray();
                foreach (var txInput in transactionBody.Collateral)
                {
                    cborCollateralInputs.Add(txInput.GetCBOR());
                }
            }
            if (cborCollateralInputs != null) cborBody.Add(13, cborCollateralInputs);

            // 14) required_signers
            if (transactionBody.RequiredSigners.Any()) {
                cborRequiredSigners = CBORObject.NewArray();
                foreach (var requireSigners in transactionBody.RequiredSigners)
                {
                    cborRequiredSigners.Add(requireSigners);
                }
            }
            if (cborRequiredSigners != null) cborBody.Add(14, cborRequiredSigners);

            // 15) network_id
            if (transactionBody.NetworkId.HasValue) 
            {
                cborBody.Add(15, transactionBody.NetworkId);
            }

            // 16) collateral return
            if (transactionBody.CollateralReturn != null) 
            {
                cborBody.Add(16, transactionBody.CollateralReturn.Serialize());
            }

            // 17) total collateral
            if (transactionBody.TotalCollateral.HasValue)
            {
                cborBody.Add(17, transactionBody.TotalCollateral);
            }

            // 18) reference inputs
            if (transactionBody.ReferenceInputs.Any()) 
            {
                cborReferenceInputs = CBORObject.NewArray();
                foreach (var referenceInputs in transactionBody.ReferenceInputs)
                {
                    cborReferenceInputs.Add(referenceInputs);
                }                
            }

            if (cborReferenceInputs != null)
            {
                cborBody.Add(18, cborReferenceInputs);
            }
            
            return cborBody;
        }

        public static TransactionBody GetTransactionBody(this CBORObject transactionBodyCbor)
        {
            //validation
            if (transactionBodyCbor == null)
            {
                throw new ArgumentNullException(nameof(transactionBodyCbor));
            }
            if (transactionBodyCbor.Type != CBORType.Map)
            {
                throw new ArgumentException("transactionBodyCbor is not expected type CBORType.Map");
            }
            if (!transactionBodyCbor.ContainsKey(0))
            {
                throw new ArgumentException("transactionBodyCbor key 0 (Inputs) not present");
            }
            if (!transactionBodyCbor.ContainsKey(1))
            {
                throw new ArgumentException("transactionBodyCbor key 1 (Outputs) not present");
            }
            if (!transactionBodyCbor.ContainsKey(2))
            {
                throw new ArgumentException("transactionBodyCbor key 2 (Fee/Coin) not present");
            }
            else if (transactionBodyCbor[2].Type != CBORType.Integer)
            {
                throw new ArgumentException("transactionBodyCbor element 2 (Fee/Coin) unexpected type (expected Integer)");
            }
            if (!transactionBodyCbor.ContainsKey(3))
            {
                throw new ArgumentException("transactionBodyCbor key 3 (TTL) not present");
            }
            else if (transactionBodyCbor[3].Type != CBORType.Integer)
            {
                throw new ArgumentException("transactionBodyCbor element 3 (TTL) unexpected type (expected Integer)");
            }

            //get data
            var transactionBody = new TransactionBody();
            //0 : set<transaction_input>    ; inputs
            var inputsCbor = transactionBodyCbor[0];
            foreach (var input in inputsCbor.Values)
            {
                transactionBody.TransactionInputs.Add(input.GetTransactionInput());
            }

            //1 : [* transaction_output]
            var outputsCbor = transactionBodyCbor[1];
            foreach (var output in outputsCbor.Values)
            {
                transactionBody.TransactionOutputs.Add(output.GetTransactionOutput());
            }

            //2 : coin                      ; fee
            transactionBody.Fee = transactionBodyCbor[2].DecodeValueToUInt64();

            //? 3 : uint                    ; time to live
            transactionBody.Ttl = transactionBodyCbor[3].DecodeValueToUInt32();

            //? 4 : [* certificate]
            if (transactionBodyCbor.ContainsKey(4))
            {
                transactionBody.Certificate = transactionBodyCbor[4].GetCertificate();
            }

            //? 5 : withdrawals
            //? 6 : update
            //? 7 : auxiliary_data_hash
            if (transactionBodyCbor.ContainsKey(7))
            {
                transactionBody.MetadataHash = (string)transactionBodyCbor[7].DecodeValueByCborType();
            }

            //? 8 : uint                    ; validity interval start
            //? 9 : mint
            if (transactionBodyCbor.ContainsKey(9))
            {
                var mintCbor = transactionBodyCbor[9];
                foreach (var key in mintCbor.Keys)
                {
                    var byteMintKey = ((string)key.DecodeValueByCborType()).HexToByteArray();
                    var assetCbor = mintCbor[key];
                    var nativeAsset = new NativeAsset();
                    foreach (var assetKey in assetCbor.Keys)
                    {
                        var byteAssetKey = ((string)assetKey.DecodeValueByCborType()).HexToByteArray();
                        var token = assetCbor[assetKey].DecodeValueToInt64();
                        nativeAsset.Token.Add(byteAssetKey, token);
                    }

                    transactionBody.Mint.Add(byteMintKey, nativeAsset);
                }
            }

            //? 11 : script_data_hash; 
            if (transactionBodyCbor.ContainsKey(11)) 
            {
                var scriptDataHashCBOR = transactionBodyCbor[11];
                var scriptDataHash = ((string)scriptDataHashCBOR.DecodeValueByCborType()).HexToByteArray();
                transactionBody.ScriptDataHash = scriptDataHash;
            }

            //? 13 : set<transaction_input> ; collateral inputs
            if (transactionBodyCbor.ContainsKey(13)) {
                var collateralInputsCbor = transactionBodyCbor[13];
                foreach (var input in inputsCbor.Values)
                {
                    transactionBody.Collateral.Add(input.GetTransactionInput());
                }
            }

            //? 14 : required_signers; 
            if (transactionBodyCbor.ContainsKey(14))
            {
                var requiredSignersCbor = transactionBodyCbor[14];
                foreach (var requiredSignerCbor in requiredSignersCbor.Values) 
                {
                    var requiredSigner = ((string)requiredSignerCbor.DecodeValueByCborType()).HexToByteArray();
                    transactionBody.RequiredSigners.Add(requiredSigner);
                }
            }

            //? 15 : network_id; 
            if (transactionBodyCbor.ContainsKey(15))
            {
                transactionBody.NetworkId = transactionBodyCbor[15].DecodeValueToUInt32();                
            }

            //? 16 : transaction_output     ; collateral return; New
            if (transactionBodyCbor.ContainsKey(16))
            {
                var collateralReturnOutputsCbor = transactionBodyCbor[16];
                foreach (var collateralReturnOutput in collateralReturnOutputsCbor.Values)
                {
                    transactionBody.TransactionOutputs.Add(collateralReturnOutput.GetTransactionOutput());
                }
            }
            
            //? 17 : coin                   ; total collateral; New
            if (transactionBodyCbor.ContainsKey(17))
            {
                transactionBody.TotalCollateral = transactionBodyCbor[17].DecodeValueToUInt64(); 
            }

            //? 18 : set<transaction_input> ; reference inputs; New
            if (transactionBodyCbor.ContainsKey(18))
            {
                var referenceInputsCbor = transactionBodyCbor[18];
                foreach (var referenceInput in referenceInputsCbor.Values)
                {
                    transactionBody.TransactionInputs.Add(referenceInput.GetTransactionInput());
                }
            }

            //return
            return transactionBody;
        }

        public static byte[] Serialize(this TransactionBody transactionBody, AuxiliaryData auxiliaryData)
        {
            return transactionBody.GetCBOR(auxiliaryData).EncodeToBytes();
        }

        public static TransactionBody DeserializeTransactionBody(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetTransactionBody();
        }
    }
}
