using CardanoSharp.Wallet.Models.Transactions;
using PeterO.Cbor2;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
