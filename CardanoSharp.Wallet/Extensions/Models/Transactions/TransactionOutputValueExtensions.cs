using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardanoSharp.Wallet.Extensions.Models.Transactions
{
    public static class TransactionOutputValueExtensions
    {
        public static CBORObject GetCBOR(this TransactionOutputValue transactionOutputValue)
        {
            if (transactionOutputValue.MultiAsset != null && transactionOutputValue.MultiAsset.Count > 0)
            {
                //add any 'coin' aka ADA to the output
                var cborAssetOutput = CBORObject.NewArray()
                    .Add(transactionOutputValue.Coin);

                var cborMultiAsset = CBORObject.NewMap();
                //iterate over the multiassets
                //reminder of this structure
                //MultiAsset = Rust Type of BTreeMap<PolicyID, Assets>
                //PolicyID = byte[](length 28)
                //Assets = BTreeMap<AssetName, uint>
                //AssetName = byte[](length 28)
                foreach (var policy in transactionOutputValue.MultiAsset)
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

                return cborAssetOutput;
            }
            else
            {
                return CBORObject.FromObject(transactionOutputValue.Coin);
            }
        }

        public static TransactionOutputValue GetTransactionOutputValue(this CBORObject transactionOutputValueCbor)
        {
            //validation
            if (transactionOutputValueCbor == null)
            {
                throw new ArgumentNullException(nameof(transactionOutputValueCbor));
            }
            if (transactionOutputValueCbor.Type != CBORType.Array && transactionOutputValueCbor.Type != CBORType.Integer)
            {
                throw new ArgumentException("transactionOutputValueCbor is not expected type CBORType.Array or CBORType.Integer");
            }
            if (transactionOutputValueCbor.Type == CBORType.Array && transactionOutputValueCbor.Count < 1)
            {
                throw new ArgumentException("transactionOutputValueCbor is array and does not contain at least 1 element (coin)");
            }
            if (transactionOutputValueCbor.Type == CBORType.Array && transactionOutputValueCbor[0].Type != CBORType.Integer)
            {
                throw new ArgumentException("transactionOutputValueCbor is array and first element (coin) is not expected type (number)");
            }

            //if its just an integer type (shelley spec) - then just populate that and return
            var outputValue = new TransactionOutputValue();
            if (transactionOutputValueCbor.Type == CBORType.Integer)
            {
                outputValue.Coin = Convert.ToUInt64(transactionOutputValueCbor.DecodeValueByCborType());
                outputValue.MultiAsset = new Dictionary<byte[], NativeAsset>();
                return outputValue;
            }

            //from here it must be an array with first element being the coin value (multi-asset spec)
            outputValue.Coin = Convert.ToUInt64(transactionOutputValueCbor[0].DecodeValueByCborType());

            //check for tokens
            if (transactionOutputValueCbor.Count > 1 && transactionOutputValueCbor[1].Type == CBORType.Map)
            {
                var muliassetCbor = transactionOutputValueCbor[1];
                outputValue.MultiAsset = new Dictionary<byte[], NativeAsset>();
                foreach(var policyCborKey in muliassetCbor.Keys)
                {
                    var policyCbor = muliassetCbor[policyCborKey];
                    var policyId = ((string)policyCborKey.DecodeValueByCborType()).HexToByteArray();
                    if (policyCbor.Type == CBORType.Map)
                    {
                        var asset = new NativeAsset();
                        foreach (var tokenCbor in policyCbor.Keys)
                        {
                            var assetBytes = ((string)tokenCbor.DecodeValueByCborType()).HexToByteArray();
                            var assetToken = Convert.ToUInt64(policyCbor[tokenCbor].DecodeValueByCborType());

                            asset.Token.Add(assetBytes, assetToken);
                        }
                        outputValue.MultiAsset.Add(policyId, asset);
                    }
                }
            }

            return outputValue;
        }

        public static byte[] Serialize(this TransactionOutputValue transactionOutputValue)
        {
            return transactionOutputValue.GetCBOR().EncodeToBytes();
        }

        public static TransactionOutputValue DeserializeTransactionOutputValue(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetTransactionOutputValue();
        }
    }
}
