using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Segments;

namespace CardanoSharp.Wallet.Models.Derivations
{
    public interface ICoinNodeDerivation : IPathDerivation
    {
        IAccountNodeDerivation Derive(int value);
    }

    public class CoinNodeDerivation : AChildKeyDerivation, ICoinNodeDerivation
    {
        public CoinNodeDerivation(PrivateKey key, CoinType value)
            : base(key, new CoinNodeSegment(value))
        {
        }

        public IAccountNodeDerivation Derive(int value)
        {
            return new AccountNodeDerivation(PrivateKey, value);
        }
    }
}
