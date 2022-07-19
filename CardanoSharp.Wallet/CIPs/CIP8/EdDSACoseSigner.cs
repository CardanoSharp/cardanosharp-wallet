using CardanoSharp.Wallet.CIPs.CIP8.Models;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using System;

namespace CardanoSharp.Wallet.CIPs.CIP8
{
    public class EdDSACoseSigner : ICoseSigner
    {
        public CoseSign1 BuildCoseSign1(byte[] payload, PrivateKey signingKey, byte[] externalAad = null, byte[] address = null)
        {
            // Build headers 
            var protectedHeaderMap = new ProtectedHeaderMap(
                new HeaderMap(AlgorithmId.EdDSA, keyId: address, address: address));
            var headers = new Headers(@protected: protectedHeaderMap);

            // Signing the CBOR bytes representation of the SigStructure wrapping around the payload
            var sigStructure = new SigStructure(
                sigContext: SigContext.Signature1,
                bodyProtected: protectedHeaderMap.Bytes,
                externalAad: externalAad ?? Array.Empty<byte>(),
                payload: payload);
            var sigStructureCborBytes = sigStructure.GetCBOR().EncodeToBytes();
            var signedSigStructure = signingKey.Sign(sigStructureCborBytes);

            return new CoseSign1(headers: headers, payload: payload, signature: signedSigStructure);
        }

        public bool VerifyCoseSign1(CoseSign1 coseSign1, PublicKey verificationKey, byte[] externalAad = null, byte[] address = null)
        {
            // Rebuild Message to verify by getting CBOR bytes representation of SigStructure 
            var sigStructure = new SigStructure(
                sigContext: SigContext.Signature1,
                bodyProtected: coseSign1.Headers.Protected.Bytes,
                payload: coseSign1.Payload,
                externalAad: externalAad);
            var sigStructureCborBytes = sigStructure.GetCBOR().EncodeToBytes();

            return verificationKey.Verify(sigStructureCborBytes, coseSign1.Signature);
        }
    }
}
