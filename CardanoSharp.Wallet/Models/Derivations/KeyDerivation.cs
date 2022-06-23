using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Segments;
using CardanoSharp.Wallet.Utilities;
using System;

namespace CardanoSharp.Wallet.Models.Derivations
{
    public interface IPathDerivation
    {
        ISegment Segment { get; }
        PrivateKey PrivateKey { get; }
        PublicKey PublicKey { get; }

        void SetPublicKey();
    }

    public abstract class AKeyDerivation : IPathDerivation
    {
        protected AKeyDerivation(ISegment segment)
        {
            Segment = segment;
        }

        public ISegment Segment { get; }
        public PrivateKey PrivateKey { get; protected set; }
        public PublicKey PublicKey { get; protected set; }

        public void SetPublicKey()
        {
            if (PrivateKey == null)
                throw new Exception("Private Key is not set");
            
            PublicKey = PrivateKey.GetPublicKey(false);
        }
    }

    public abstract class AChildKeyDerivation : AKeyDerivation, IPathDerivation
    {
        const uint MinHardIndex = 0x80000000;


        protected AChildKeyDerivation(PrivateKey key, ISegment segment) : base(segment)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var index = Convert.ToUInt32(segment.Value);
            if (segment.IsHardened) index += MinHardIndex;
            PrivateKey = Bip32Utility.GetChildKeyDerivation(key, index);
            PublicKey = PrivateKey.GetPublicKey(withZeroByte: false);
        }

        protected AChildKeyDerivation(PublicKey key, ISegment segment) : base(segment)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var index = Convert.ToUInt32(segment.Value);
            if (segment.IsHardened) throw new Exception("Public Keys cannot derive hardened paths");
            PrivateKey = null;
            PublicKey = Bip32Utility.GetChildKeyDerivation(key, index);
        }
    }
}
