using System.Collections.Generic;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies
{
    public interface IChangeCreationStrategy
    {
        void CalculateChange(CoinSelection coinSelection, List<Asset> assets);
    }
}