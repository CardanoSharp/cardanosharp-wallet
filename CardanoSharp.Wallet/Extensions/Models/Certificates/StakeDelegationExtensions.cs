using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;

namespace CardanoSharp.Wallet.Extensions.Models.Certificates
{
    public static class StakeDelegationExtensions
    {
        public static CBORObject GetCBOR(this StakeDelegation stakeDelegation)
        {
            return CBORObject.NewArray()
                .Add(2)
                .Add(CBORObject.NewArray()
                    .Add(0)
                    .Add(stakeDelegation.StakeCredential)
                )
                .Add(stakeDelegation.PoolHash);
        }

        public static StakeDelegation GetStakeDelegation(this CBORObject stakeDelegationCbor)
        {
            //validate
            if (stakeDelegationCbor == null)
            {
                throw new ArgumentNullException(nameof(stakeDelegationCbor));
            }
            if (stakeDelegationCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("stakeDelegationCbor is not expected type CBORType.Array");
            }
            if (stakeDelegationCbor.Values.Count != 3)
            {
                throw new ArgumentException("stakeDelegationCbor has unexpected number of elements (expected 3)");
            }
            if (stakeDelegationCbor[0].DecodeValueToInt16() != 2)
            {
                throw new ArgumentException("stakeDelegationCbor first element has unexpected value (expected 2)");
            }
            var credentialCbor = stakeDelegationCbor[1];
            if (credentialCbor.Type != CBORType.Array)
            {
                throw new ArgumentException("credentialCbor is not expected type CBORType.Array");
            }
            if (credentialCbor.Count != 2)
            {
                throw new ArgumentException("credentialCbor has unexpected number of elements (expected 2)");
            }
            if (credentialCbor[0].DecodeValueToInt16() != 0)
            {
                throw new ArgumentException("credentialCbor first element has unexpected value (expected 0)");
            }

            //populate
            var stakeDelegation = new StakeDelegation();
            stakeDelegation.StakeCredential = ((string)credentialCbor[1].DecodeValueByCborType()).HexToByteArray();
            stakeDelegation.PoolHash = ((string)stakeDelegationCbor[2].DecodeValueByCborType()).HexToByteArray();

            //return
            return stakeDelegation;
        }

        public static byte[] Serialize(this StakeDelegation stakeDelegation)
        {
            return stakeDelegation.GetCBOR().EncodeToBytes();
        }

        public static StakeDelegation DeserializeStakeDelegation(this byte[] bytes)
        {
            return CBORObject.DecodeFromBytes(bytes).GetStakeDelegation();
        }
    }
}
