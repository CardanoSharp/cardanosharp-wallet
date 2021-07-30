namespace CardanoSharp.Wallet.Models.Segments
{
    public class MasterNodeSegment : ASegment
    {
        public MasterNodeSegment(char value = 'm') : base(value, root: true)
        {
        }
    }
}
