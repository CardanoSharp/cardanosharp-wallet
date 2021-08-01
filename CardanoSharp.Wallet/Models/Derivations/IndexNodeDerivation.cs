using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Segments;

namespace CardanoSharp.Wallet.Models.Derivations
{
    public interface IIndexNodeDerivation : IPathDerivation
    {
    }
    public class IndexNodeDerivation : AChildKeyDerivation, IIndexNodeDerivation
    {
        public IndexNodeDerivation(PrivateKey key, int value) : base(key, new IndexNodeSegment(value))
        {
        }

        public IndexNodeDerivation(PublicKey key, int value) : base(key, new IndexNodeSegment(value))
        {
        }
    }
}
