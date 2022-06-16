using System.Collections.Generic;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Transactions;

namespace CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies
{
    public interface IChangeCreationStrategy
    {
        void CalculateChange(CoinSelection coinSelection, Balance balance);
    }
}