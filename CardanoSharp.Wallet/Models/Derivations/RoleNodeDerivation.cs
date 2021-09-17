using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Segments;

namespace CardanoSharp.Wallet.Models.Derivations
{
    public interface IRoleNodeDerivation : IPathDerivation
    {
        IIndexNodeDerivation Derive(int value);
    }
    public class RoleNodeDerivation : AChildKeyDerivation, IRoleNodeDerivation
    {
        public RoleNodeDerivation(PrivateKey key, RoleType value) : base(key, new RoleNodeSegment(value))
        {
        }

        public RoleNodeDerivation(PublicKey key, RoleType value) : base(key, new RoleNodeSegment(value))
        {
        }

        public IIndexNodeDerivation Derive(int value)
        {
            if(PrivateKey == null)
            {
                return new IndexNodeDerivation(PublicKey, value);
            }
            return new IndexNodeDerivation(PrivateKey, value);
        }
    }
}
