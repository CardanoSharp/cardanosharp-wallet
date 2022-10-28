using System.Collections.Generic;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.TransactionBuilding;

namespace CardanoSharp.Wallet.CIPs.CIP2.ChangeCreationStrategies
{
    public interface IChangeCreationStrategy
    {
        void CalculateChange(CoinSelection coinSelection, Balance balance, ITokenBundleBuilder mint = null);
    }
}