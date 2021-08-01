using CardanoSharp.Wallet.Enums;

namespace CardanoSharp.Wallet.Models.Segments
{
    public class RoleNodeSegment : ASegment
    {
        public RoleNodeSegment(RoleType value) : base(value, derivation: DerivationType.SOFT)
        {
        }
    }
}
