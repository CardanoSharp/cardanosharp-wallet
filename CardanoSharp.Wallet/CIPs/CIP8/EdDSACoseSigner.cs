using CardanoSharp.Wallet.CIPs.CIP8.Models;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using System;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.CIPs.CIP8
{
    public class EdDsaCoseSigner : ICoseSigner
    {
        public CoseSign1 BuildCoseSign1(
            byte[] payload, PrivateKey signingKey, byte[] externalAad = null, byte[] address = null, bool hashed = false)
        {
            // Build headers 
            var protectedHeaderMap = new ProtectedHeaderMap(
                new HeaderMap(AlgorithmId.EdDsa, address: address));

            // following cardano-signer example. 
            // https://github.com/gitmachtl/cardano-signer/blob/e792768944c8f8d5244a418472cc14028e95aa27/src/cardano-signer.js#L608
            var unprotected = new HeaderMap(otherHeaders: new Dictionary<object, object>
            {
                {"hashed", hashed}
            });

            var headers = new Headers(@protected: protectedHeaderMap, unprotected);

            // Signing the CBOR bytes representation of the SigStructure wrapping around the payload
            var sigStructure = new SigStructure(
                sigContext: SigContext.Signature1,
                bodyProtected: protectedHeaderMap.Bytes,
                externalAad: externalAad ?? Array.Empty<byte>(),
                payload: payload);
            var sigStructureCborBytes = sigStructure.GetCbor().EncodeToBytes();
            var signedSigStructure = signingKey.Sign(sigStructureCborBytes);

            return new CoseSign1(headers: headers, payload: payload, signature: signedSigStructure);
        }

        public bool VerifyCoseSign1(
            CoseSign1 coseSign1, PublicKey verificationKey, byte[] externalAad = null, byte[] address = null)
        {
            // Rebuild Message to verify by getting CBOR bytes representation of SigStructure 
            var sigStructure = new SigStructure(
                sigContext: SigContext.Signature1,
                bodyProtected: coseSign1.Headers.Protected.Bytes,
                payload: coseSign1.Payload,
                externalAad: externalAad);
            var sigStructureCborBytes = sigStructure.GetCbor().EncodeToBytes();

            return verificationKey.Verify(sigStructureCborBytes, coseSign1.Signature);
        }

    }
}
