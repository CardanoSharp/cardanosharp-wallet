using System;
using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.NativeScripts;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static partial class TransactionOutputExtensions
    {
        private static ulong adaOnlyMinUTxO = 1000000;
        private static string dummyAddress =
            "addr_test1qpu5vlrf4xkxv2qpwngf6cjhtw542ayty80v8dyr49rf5ewvxwdrt70qlcpeeagscasafhffqsxy36t90ldv06wqrk2qum8x5w";

        public static CBORObject GetCBOR(this TransactionOutput transactionOutput)
        {
            // Support Legacy Alonzo Transaction format
            if (transactionOutput.DatumOption == null && transactionOutput.ScriptReference == null)
            {
                CBORObject legacyTransactionCBOR = CBORObject
                .NewArray()
                .Add(transactionOutput.Address)
                .Add(transactionOutput.Value.GetCBOR());
                return legacyTransactionCBOR;
            }

            CBORObject cborTransactionOutput = CBORObject
                .NewMap()
                .Add(0, transactionOutput.Address)
                .Add(1, transactionOutput.Value.GetCBOR());

            if (transactionOutput.DatumOption is not null)
            {
                var cborDatumOption = CBORObject.NewArray();
                if (transactionOutput.DatumOption.Hash is not null)
                {
                    cborDatumOption.Add(0);
                    cborDatumOption.Add(transactionOutput.DatumOption.Hash);
                }
                else if (transactionOutput.DatumOption.Data is not null)
                {
                    cborDatumOption.Add(1);
                    cborDatumOption.Add(transactionOutput.DatumOption.GetCBOR());
                }

                cborTransactionOutput.Add(2, cborDatumOption);
            }

            if (transactionOutput.ScriptReference is not null)
            {
                var cborScriptReference = transactionOutput.ScriptReference.Serialize();
                cborTransactionOutput.Add(3, cborScriptReference);
            }

            return cborTransactionOutput;
        }

        public static TransactionOutput GetTransactionOutput(this CBORObject transactionOutputCbor)
        {
            //validation
            if (transactionOutputCbor == null)
            {
                throw new ArgumentNullException(nameof(transactionOutputCbor));
            }
            if (
                transactionOutputCbor.Type != CBORType.Map
                && transactionOutputCbor.Type != CBORType.Array
            ) // We must support both pre and post alonzo format
            {
                throw new ArgumentException(
                    "transactionOutputCbor is not expected type CBORType.Map"
                );
            }
            if (transactionOutputCbor.Count < 2)
            {
                throw new ArgumentException(
                    "transactionOutputCbor unexpected number elements (expected at least 2)"
                );
            }
            if (transactionOutputCbor[0].Type != CBORType.ByteString)
            {
                throw new ArgumentException(
                    "transactionOutputCbor first element unexpected type (expected ByteString)"
                );
            }
            if (
                transactionOutputCbor[1].Type != CBORType.Integer
                && transactionOutputCbor[1].Type != CBORType.Array
            )
            {
                throw new ArgumentException(
                    "transactionInputCbor second element unexpected type (expected Integer or Array)"
                );
            }

            //get data
            var transactionOutput = new TransactionOutput();
            transactionOutput.Address = (
                (string)transactionOutputCbor[0].DecodeValueByCborType()
            ).HexToByteArray();
            if (transactionOutputCbor[1].Type == CBORType.Integer)
            {
                //coin
                transactionOutput.Value = new TransactionOutputValue()
                {
                    Coin = transactionOutputCbor[1].DecodeValueToUInt64()
                };
            }
            else
            {
                //multi asset
                transactionOutput.Value = new TransactionOutputValue();
                transactionOutput.Value.MultiAsset = new Dictionary<byte[], NativeAsset>();

                var coinCbor = transactionOutputCbor[1][0];
                transactionOutput.Value.Coin = coinCbor.DecodeValueToUInt64();

                var multiAssetCbor = transactionOutputCbor[1][1];
                foreach (var policyKeyCbor in multiAssetCbor.Keys)
                {
                    var nativeAsset = new NativeAsset();

                    var assetMapCbor = multiAssetCbor[policyKeyCbor];
                    var policyKeyBytes = (
                        (string)policyKeyCbor.DecodeValueByCborType()
                    ).HexToByteArray();

                    foreach (var assetKeyCbor in assetMapCbor.Keys)
                    {
                        var assetToken = assetMapCbor[assetKeyCbor].DecodeValueToInt64();
                        var assetKeyBytes = (
                            (string)assetKeyCbor.DecodeValueByCborType()
                        ).HexToByteArray();

                        nativeAsset.Token.Add(assetKeyBytes, assetToken);
                    }

                    transactionOutput.Value.MultiAsset.Add(policyKeyBytes, nativeAsset);
                }
            }

            // Datum Option, this does not support legacy V1 datum hash
            if (transactionOutputCbor.ContainsKey(2))
            {
                // How do we support datum hash here as well for contracts with secret data?
                var datumOptionCbor = transactionOutputCbor[2];

                // Does this work?
                IPlutusData plutusData = (IPlutusData)datumOptionCbor.DecodeValueByCborType();

                DatumOption datumOption = new DatumOption { Data = plutusData };
                transactionOutput.DatumOption = (DatumOption?)datumOption;
            }

            // Script Reference
            if (transactionOutputCbor.ContainsKey(3))
            {
                ScriptReference scriptReference = new ScriptReference();
                var scriptReferenceCborWithTag = transactionOutputCbor[3];

                if (scriptReferenceCborWithTag.HasMostOuterTag(24))
                {
                    var scriptReferenceCbor = scriptReferenceCborWithTag.Untag();

                    // Must decode the object after removing the tag
                    var decodedScriptReferenceCbor = CBORObject.DecodeFromBytes(
                        ((string)scriptReferenceCbor.DecodeValueByCborType()).HexToByteArray()
                    );
                    var scriptKey = decodedScriptReferenceCbor[0].DecodeValueToUInt32();
                    if (scriptKey == 0)
                    {
                        // Is this correct for native script?
                        NativeScript nativeScript = (NativeScript)
                            decodedScriptReferenceCbor[1].DecodeValueByCborType();
                        scriptReference.NativeScript = nativeScript;
                    }
                    else if (scriptKey == 1)
                    {
                        byte[] plutusV1ScriptBytes = (
                            (string)scriptReferenceCbor[1].DecodeValueByCborType()
                        ).HexToByteArray();
                        PlutusV1Script plutusV1Script = new PlutusV1Script
                        {
                            script = plutusV1ScriptBytes
                        };
                        scriptReference.PlutusV1Script = plutusV1Script;
                    }
                    else if (scriptKey == 2)
                    {
                        byte[] plutusV2ScriptBytes = (
                            (string)decodedScriptReferenceCbor[1].DecodeValueByCborType()
                        ).HexToByteArray();
                        PlutusV2Script plutusV2Script = new PlutusV2Script
                        {
                            script = plutusV2ScriptBytes
                        };
                        scriptReference.PlutusV2Script = plutusV2Script;
                    }
                    transactionOutput.ScriptReference = scriptReference;
                }
            }

            //return
            return transactionOutput;
        }

        public static byte[] Serialize(this TransactionOutput transactionOutput)
        {
            return transactionOutput.GetCBOR().EncodeToBytes();
        }

        public static TransactionOutput DeserializeTransactionOutput(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetTransactionOutput();
        }

        public static ulong CalculateMinUtxoLovelace(
            this TransactionOutput output,
            ulong coinsPerUtxOByte = 4310 // coinsPerUtxoByte in protocol params
        )
        {
            if (output.Value.MultiAsset == null || output.Value.MultiAsset.Count <= 0)
                return adaOnlyMinUTxO;

            // Set a dummy coin value if coin is 0
            bool setDummyCoin = false;
            if (output.Value.Coin == 0)
            {
                setDummyCoin = true;
                output.Value.Coin = adaOnlyMinUTxO;
            }

            // Set a dummy address if this function is called with Address == null
            if (output.Address == null)
                output.Address = new Address(dummyAddress).GetBytes();

            byte[] serializedOutput = output.Serialize();
            ulong outputLength = (ulong)serializedOutput.Length;
            ulong minUTxO = coinsPerUtxOByte * (160 + outputLength);
            if (minUTxO < adaOnlyMinUTxO)
                minUTxO = adaOnlyMinUTxO;

            if (setDummyCoin)
            {
                output.Value.Coin = 0;
            }

            return minUTxO;
        }

        //maxOutputBytesSize is a Protocol Parameter and may change in the future
        public static bool IsValid(this TransactionOutput output, ulong maxOutputBytesSize = 5000)
        {
            byte[] serializedOutput = output.Serialize();
            ulong outputLength = (ulong)serializedOutput.Length;
            if (outputLength > maxOutputBytesSize)
                return false;
            return true;
        }
    }
}
