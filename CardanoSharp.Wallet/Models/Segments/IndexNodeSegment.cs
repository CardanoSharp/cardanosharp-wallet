using CardanoSharp.Wallet.Enums;

namespace CardanoSharp.Wallet.Models.Segments
{
    public class IndexNodeSegment : ASegment
    {
        public IndexNodeSegment(int value) : base(value, derivation: DerivationType.SOFT)
        {
        }
    }
}
