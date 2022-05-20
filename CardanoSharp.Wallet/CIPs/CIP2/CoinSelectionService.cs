using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public class CoinSelectionService
    {
        private readonly ICoinSelectionStrategy _coinSelection;

        public CoinSelectionService(ICoinSelectionStrategy coinSelection)
        {
            _coinSelection = coinSelection;
        }

        public List<TransactionUnspentOutput> GetInputs(List<TransactionOutput> outputs, List<TransactionUnspentOutput> utxos, ulong amount, Asset asset = null)
        {
            var selectedUtxos = _coinSelection.SelectInputs(utxos, amount, asset);

            if (!HasSufficientBalance(selectedUtxos, amount, asset))
                throw new Exception($"UTxOs have insufficient balance");

            return selectedUtxos;
        }

        private bool HasSufficientBalance(List<TransactionUnspentOutput> selectedUtxos, ulong amount, Asset asset = null)
        {
            ulong totalInput = 0;
            foreach (var su in selectedUtxos)
            {
                ulong quantity = 0;
                if (asset is null)
                {
                    quantity = su.Output.Value.Coin;
                }else
                {
                    var hasAsset = su.Output.Value.MultiAsset
                        .Any(ma => 
                            ma.Key.SequenceEqual(asset.PolicyId)
                            && ma.Value.Token.ContainsKey(asset.Name));

                    if (!hasAsset) throw new Exception($"UTxOs do not have contain a required asset: {asset.Name.ToStringHex()}");

                    quantity = su.Output.Value.MultiAsset
                        .First(ma =>
                            ma.Key.SequenceEqual(asset.PolicyId)
                            && ma.Value.Token.ContainsKey(asset.Name))
                        .Value.Token[asset.Name];
                }
                
                totalInput = totalInput + quantity;
            }

            return totalInput > amount;
        }
    }
}