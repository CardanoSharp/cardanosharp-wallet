using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static class TransactionOutputExtensions
    {
        public static CBORObject GetCBOR(this TransactionOutput transactionOutput)
        {
            //start the cbor transaction output object with the address we are sending
            var cborTransactionOutput = CBORObject.NewArray()
                .Add(transactionOutput.Address);

            //determine if the output has any native assets included
            if (transactionOutput.Value.MultiAsset != null && transactionOutput.Value.MultiAsset.Count != 0)
            {
                //add any 'coin' aka ADA to the output
                var cborAssetOutput = CBORObject.NewArray()
                    .Add(transactionOutput.Value.Coin);

                var cborMultiAsset = CBORObject.NewMap();

                //iterate over the multiassets
                //reminder of this structure
                //MultiAsset = Rust Type of BTreeMap<PolicyID, Assets>
                //PolicyID = byte[](length 28)
                //Assets = BTreeMap<AssetName, uint>
                //AssetName = byte[](length 28)
                foreach (var policy in transactionOutput.Value.MultiAsset)
                {
                    //in this scope
                    //policy.Key = PolicyID
                    //policy.Values = Assets

                    var assetMap = CBORObject.NewMap();
                    foreach (var asset in policy.Value.Token)
                    {
                        //in this scope
                        //asset.Key = AssetName
                        //asset.Value = uint
                        assetMap.Add(asset.Key, asset.Value);
                    }

                    //add our PolicyID (policy.Key) and Assets (assetMap) to cborTokenOutput
                    cborMultiAsset.Add(policy.Key, assetMap);
                }

                //Add the multi asset to the assets
                cborAssetOutput.Add(cborMultiAsset);

                //finally add our assetOutput to our transaction output
                cborTransactionOutput.Add(cborAssetOutput);
            }
            else
            {
                //heres a simple send ada transaction
                cborTransactionOutput.Add(transactionOutput.Value.Coin);
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
            if (transactionOutputCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("transactionOutputCbor is not expected type CBORType.Map");
            }
            if (transactionOutputCbor.Count != 2)
            {
                throw new ArgumentException("transactionOutputCbor unexpected number elements (expected 2)");
            }
            if (transactionOutputCbor[0].Type != CBORType.ByteString)
            {
                throw new ArgumentException("transactionOutputCbor first element unexpected type (expected ByteString)");
            }
            if (transactionOutputCbor[1].Type != CBORType.Integer && transactionOutputCbor[1].Type != CBORType.Array)
            {
                throw new ArgumentException("transactionInputCbor second element unexpected type (expected Integer or Array)");
            }

            //get data
            var transactionOutput = new TransactionOutput();
            transactionOutput.Address = ((string)transactionOutputCbor[0].DecodeValueByCborType()).HexToByteArray();
            if (transactionOutputCbor[1].Type == CBORType.Integer)
            {
                //coin
                transactionOutput.Value = new TransactionOutputValue() 
                { 
                    Coin = Convert.ToUInt64(transactionOutputCbor[1].DecodeValueByCborType()) 
                };
            }
            else
            {
                //multi asset
                transactionOutput.Value = new TransactionOutputValue();
                transactionOutput.Value.MultiAsset = new Dictionary<byte[], NativeAsset<ulong>>();

                var coinCbor = transactionOutputCbor[1][0];
                transactionOutput.Value.Coin = Convert.ToUInt64(coinCbor.DecodeValueByCborType());

                var multiAssetCbor = transactionOutputCbor[1][1];
                foreach (var policyKeyCbor in multiAssetCbor.Keys)
                {
                    var nativeAsset = new NativeAsset<ulong>();

                    var assetMapCbor = multiAssetCbor[policyKeyCbor];
                    var policyKeyBytes = ((string)policyKeyCbor.DecodeValueByCborType()).HexToByteArray();

                    foreach (var assetKeyCbor in assetMapCbor.Keys)
                    {
                        var assetToken = Convert.ToUInt32(assetMapCbor[assetKeyCbor].DecodeValueByCborType());
                        var assetKeyBytes = ((string)assetKeyCbor.DecodeValueByCborType()).HexToByteArray();

                        nativeAsset.Token.Add(assetKeyBytes, assetToken);
                    }

                    transactionOutput.Value.MultiAsset.Add(policyKeyBytes, nativeAsset);
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
    }
}
