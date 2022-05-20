using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static byte[] Serialize(this TransactionOutput transactionOutput)
        {
            return transactionOutput.GetCBOR().EncodeToBytes();
        }

        public static List<Asset> AggregateAssets(this List<TransactionOutput> transactionOutputs)
        {
            List<Asset> assets = new List<Asset>();

            foreach (var o in transactionOutputs)
            {
                //aggregate lovelaces
                var assetLovelace = assets.FirstOrDefault(x => x.Name is null);
                if (assetLovelace is null)
                {
                    assetLovelace = new Asset()
                    {
                        Quantity = 0
                    };
                    assets.Add(assetLovelace);
                }

                assetLovelace.Quantity = assetLovelace.Quantity + o.Value.Coin;

                //aggregate native assets
                foreach (var ma in o.Value.MultiAsset)
                {
                    foreach (var na in ma.Value.Token)
                    {
                        var nativeAsset = assets.FirstOrDefault(x =>
                            x.PolicyId.SequenceEqual(ma.Key) && x.Name.SequenceEqual(na.Key));
                        if (nativeAsset is null)
                        {
                            nativeAsset = new Asset()
                            {
                                PolicyId = ma.Key,
                                Name = na.Key,
                                Quantity = 0
                            };
                            assets.Add(nativeAsset);
                        }

                        nativeAsset.Quantity = nativeAsset.Quantity + na.Value;
                    }
                }
            }
            
            return assets;
        }
    }
}
