using PeterO.Cbor2;
using System;


/// <summary>
/// https://cips.cardano.org/cips/cip8/
/// Signer: Build SigStructure, Sign SigStructure CBOR bytes with Ed25519 Key, Build CoseSign1 Result around it
/// Verifier: Extract Payload & Headers from CoseSign1, Use VKey to verify Signature 
/// Also useful: https://cips.cardano.org/cips/cip30/#apisigndataaddraddresspayloadbytespromisedatasignature
/// </summary>
namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    public enum SigContext
    {
        Signature, Signature1, CounterSignature
    }

    public class SigStructure
    {
        public SigContext SigContext { get; }
        public byte[] BodyProtected { get; } // Protected Header bytestring
        public byte[] SignProtected { get; } // Not present in CoseSign1
        public byte[] ExternalAad { get; }
        public byte[] Payload { get; }

        public SigStructure(
            SigContext sigContext, 
            byte[] bodyProtected, 
            byte[] signProtected = null, 
            byte[] externalAad = null, 
            byte[] payload = null)
        {
            if (SigContext == SigContext.Signature1 && SignProtected is not null)
                throw new ArgumentException($"{nameof(signProtected)} should not be present for Signature1");
            SigContext = sigContext;
            BodyProtected = bodyProtected;
            SignProtected = signProtected;
            ExternalAad = externalAad;
            Payload = payload;
        }

        public CBORObject GetCBOR()
        {
            var cbor = CBORObject.NewArray();
            cbor.Add(SigContext.ToString());
            cbor.Add(BodyProtected);
            if (SignProtected != null)
            {
                cbor.Add(SignProtected);
            }
            cbor.Add(ExternalAad ?? Array.Empty<byte>());
            cbor.Add(Payload ?? Array.Empty<byte>());
            return cbor;
        }
    }
}