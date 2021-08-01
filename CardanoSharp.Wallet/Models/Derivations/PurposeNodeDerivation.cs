using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Segments;

namespace CardanoSharp.Wallet.Models.Derivations
{
    public interface IPurposeNodeDerivation : IPathDerivation
    {
        ICoinNodeDerivation Derive(CoinType value = CoinType.Ada);
    }
    public class PurposeNodeDerivation : AChildKeyDerivation, IPurposeNodeDerivation
    {
        public PurposeNodeDerivation(PrivateKey key, PurposeType value)
            : base(key, new PurposeNodeSegment(value))
        {
        }

        public ICoinNodeDerivation Derive(CoinType value = CoinType.Ada)
        {
            return new CoinNodeDerivation(PrivateKey, value);
        }
    }
}
