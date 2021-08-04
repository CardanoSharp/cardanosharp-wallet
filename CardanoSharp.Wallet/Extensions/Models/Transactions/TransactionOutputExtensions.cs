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
            if (transactionOutput.Value.MultiAsset != null)
            {
                //add any 'coin' aka ADA to the output
                var cborAssetOutput = CBORObject.NewArray()
                    .Add(transactionOutput.Value.Coin);

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

                    //add our PolicyID (policy.Key) and Assets (assetMap)
                    var multiassetMap = CBORObject.NewMap()
                        .Add(policy.Key, assetMap);

                    //add our multiasset to our assetOutput
                    cborAssetOutput.Add(multiassetMap);
                }

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
    }
}
