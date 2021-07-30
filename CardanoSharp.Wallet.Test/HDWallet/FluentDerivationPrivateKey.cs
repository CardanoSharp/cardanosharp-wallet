using CardanoSharp.Wallet.Models.Derivations;
using CardanoSharp.Wallet.Models.Keys;

namespace CardanoSharp.Wallet.Test
{
    public class FluentDerivationPrivateKey : PrivateKey
    {
        public FluentDerivationPrivateKey(byte[] key, byte[] chaincode) : base(key, chaincode)
        {
        }

        public IMasterNodeDerivation Derive()
        {
            return new MasterNodeDerivation(this);
        }
    }
}
