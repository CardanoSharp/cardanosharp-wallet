
using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;


namespace CardanoSharp.Wallet.Extensions
{
    public static class NativeAssetCollectionExtension
    {
        public static ulong CalculateMinUtxoLovelace(this Dictionary<byte[], NativeAsset> tokens)
        {
            TransactionOutput transactionOutput = new TransactionOutput { Value = new TransactionOutputValue { MultiAsset = tokens }};
            return transactionOutput.CalculateMinUtxoLovelace();
        }
    }
}
