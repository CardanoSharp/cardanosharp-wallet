using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Segments;

namespace CardanoSharp.Wallet.Models.Derivations
{
    public interface IAccountNodeDerivation : IPathDerivation
    {
        IRoleNodeDerivation Derive(RoleType value);
    }
    public class AccountNodeDerivation : AChildKeyDerivation, IAccountNodeDerivation
    {
        public AccountNodeDerivation(PrivateKey key, int value)
            : base(key, new AccountNodeSegment(value)) { }

        public IRoleNodeDerivation Derive(RoleType value)
        {
            if (PrivateKey == null)
            {
                return new RoleNodeDerivation(PublicKey, value);
            }
            return new RoleNodeDerivation(PrivateKey, value);
        }
    }
}
