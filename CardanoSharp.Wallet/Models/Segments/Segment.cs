using CardanoSharp.Wallet.Enums;
using System;

namespace CardanoSharp.Wallet.Models.Segments
{
    public interface ISegment
    {
        IConvertible Value { get; }
        //string Name { get; }
        DerivationType? Derivation { get; }
        bool IsRoot { get; }
        bool IsHardened { get; }

        string ToString();
    }

    public abstract class ASegment : ISegment
    {
        private DerivationType _derivation;

        public ASegment(IConvertible value, DerivationType derivation = DerivationType.HARD, bool root = false)
        {
            IsRoot = root;
            Value = value;
            if (root) return;
            _derivation = derivation;
        }

        public bool IsRoot { get; }
        public string Name { get; }
        public virtual DerivationType? Derivation => _derivation;

        public bool IsHardened => Derivation == DerivationType.HARD;

        public IConvertible Value { get; }

        public override string ToString()
        {
            return (IsRoot ? Value : Convert.ToInt32(Value)).ToString();
        }
    }
}
