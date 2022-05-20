using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2
{
    public interface ILargestFirstStrategy: ICoinSelectionStrategy
    {
        
    }

    public class LargestFirstStrategy: ILargestFirstStrategy
    {
        public List<TransactionUnspentOutput> SelectInputs(List<TransactionUnspentOutput> utxos, ulong amount, Asset asset = null)
        {
            throw new System.NotImplementedException();
        }
    }
}