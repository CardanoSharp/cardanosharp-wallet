using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Segments;
using System;

namespace CardanoSharp.Wallet.Models.Derivations
{
    public interface IMasterNodeDerivation : IPathDerivation
    {
        IPurposeNodeDerivation Derive(PurposeType value = PurposeType.Shelley);
    }

    public class MasterNodeDerivation : AKeyDerivation, IMasterNodeDerivation
    {
        public MasterNodeDerivation(PrivateKey key) : base(new MasterNodeSegment())
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            PrivateKey = new(key.Key, key.Chaincode);
            PublicKey = PrivateKey.GetPublicKey();
        }

        public IPurposeNodeDerivation Derive(PurposeType value)
        {
            return new PurposeNodeDerivation(PrivateKey, value);
        }
    }
}
