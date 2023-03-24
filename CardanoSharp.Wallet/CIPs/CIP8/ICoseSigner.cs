using CardanoSharp.Wallet.CIPs.CIP8.Models;
using CardanoSharp.Wallet.Models.Keys;

namespace CardanoSharp.Wallet.CIPs.CIP8
{
    public interface ICoseSigner
    {
        CoseSign1 BuildCoseSign1(
            byte[] payload, PrivateKey signingKey, byte[] externalAad = null, byte[] address = null);

        bool VerifyCoseSign1(
            CoseSign1 coseSign1, PublicKey verificationKey, byte[] externalAad = null, byte[] address = null);
    }
}
