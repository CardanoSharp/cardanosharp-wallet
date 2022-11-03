
using System;
using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Models.Transactions.Scripts;
using CardanoSharp.Wallet.Utilities;

namespace CardanoSharp.Wallet
{
    [Obsolete("See AddressUtility")]
    public interface IAddressService
    {
        [Obsolete]
        Address GetAddress(PublicKey payment, PublicKey stake, NetworkType networkType, AddressType addressType);
        [Obsolete("See AddressUtility.GetBaseAddress")]
        Address GetBaseAddress(PublicKey payment, PublicKey stake, NetworkType networkType);
        [Obsolete("See AddressUtility.GetBaseScriptAddress")]
        Address GetBaseScriptAddress<T,K>(T paymentPolicy, K stakePolicy, NetworkType networkType);
        [Obsolete("See AddressUtility.GetRewardAddress")]
        Address GetRewardAddress(PublicKey stake, NetworkType networkType);
        [Obsolete("See AddressUtility.GetEnterpriseAddress")]
        Address GetEnterpriseAddress(PublicKey payment, NetworkType networkType);
        [Obsolete("See AddressUtility.GetEnterpriseScriptAddress")]
        Address GetEnterpriseScriptAddress<T>(T paymentPolicy, NetworkType networkType);
        [Obsolete("Use address.ExtractRewardAddress()")]
        Address ExtractRewardAddress(Address basePaymentAddress);
    }
    [Obsolete("See AddressUtility")]
    public class AddressService : IAddressService
    {
        [Obsolete("This method has been broken up. Please see other GetAddress methods.")]
        public Address GetAddress(PublicKey payment, PublicKey stake, NetworkType networkType, AddressType addressType)
        {
            var networkInfo = AddressUtility.GetNetworkInfo(networkType);
            var paymentEncoded = HashUtility.Blake2b224(payment.Key);
            var stakeEncoded = HashUtility.Blake2b224(stake.Key);

            //get prefix
            var prefix = $"{AddressUtility.GetPrefixHeader(addressType)}{AddressUtility.GetPrefixTail(networkType)}";

            //get header
            var header = AddressUtility.GetHeader(networkInfo, addressType);
            //get body
            byte[] addressArray;
            switch (addressType)
            {
                case AddressType.Base:
                    addressArray = new byte[1 + paymentEncoded.Length + stakeEncoded.Length];
                    addressArray[0] = header;
                    Buffer.BlockCopy(paymentEncoded, 0, addressArray, 1, paymentEncoded.Length);
                    Buffer.BlockCopy(stakeEncoded, 0, addressArray, paymentEncoded.Length + 1, stakeEncoded.Length);
                    break;
                case AddressType.Enterprise:
                    addressArray = new byte[1 + paymentEncoded.Length];
                    addressArray[0] = header;
                    Buffer.BlockCopy(paymentEncoded, 0, addressArray, 1, paymentEncoded.Length);
                    break;
                case AddressType.Reward:
                    addressArray = new byte[1 + stakeEncoded.Length];
                    addressArray[0] = header;
                    Buffer.BlockCopy(stakeEncoded, 0, addressArray, 1, stakeEncoded.Length);
                    break;
                default:
                    throw new Exception("Unknown address type");
            }

            return new Address(prefix, addressArray);
        }

        [Obsolete("See AddressUtility.GetBaseAddress")]
        public Address GetBaseAddress(PublicKey payment, PublicKey stake, NetworkType networkType)
        {
            var addressType = AddressType.Base;
            var networkInfo = AddressUtility.GetNetworkInfo(networkType);
            var paymentEncoded = HashUtility.Blake2b224(payment.Key);
            var stakeEncoded = HashUtility.Blake2b224(stake.Key);

            //get prefix
            var prefix = $"{AddressUtility.GetPrefixHeader(addressType)}{AddressUtility.GetPrefixTail(networkType)}";

            //get header
            var header = AddressUtility.GetHeader(networkInfo, addressType);
            
            //get body
            byte[] addressArray = new byte[1 + paymentEncoded.Length + stakeEncoded.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(paymentEncoded, 0, addressArray, 1, paymentEncoded.Length);
            Buffer.BlockCopy(stakeEncoded, 0, addressArray, paymentEncoded.Length + 1, stakeEncoded.Length);

            return new Address(prefix, addressArray);
        }

        [Obsolete("See AddressUtility.GetRewardAddress")]
        public Address GetRewardAddress(PublicKey stake, NetworkType networkType)
        {
            var stakeEncoded = HashUtility.Blake2b224(stake.Key);
            var addressType = AddressType.Reward;
            var networkInfo = AddressUtility.GetNetworkInfo(networkType);

            //get prefix
            var prefix = $"{AddressUtility.GetPrefixHeader(addressType)}{AddressUtility.GetPrefixTail(networkType)}";

            //get header
            var header = AddressUtility.GetHeader(networkInfo, addressType);
            
            //get body
            byte[] addressArray = new byte[1 + stakeEncoded.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(stakeEncoded, 0, addressArray, 1, stakeEncoded.Length);

            return new Address(prefix, addressArray);
        }

        [Obsolete("See AddressUtility.GetEnterpriseAddress")]
        public Address GetEnterpriseAddress(PublicKey payment, NetworkType networkType)
        {
            var addressType = AddressType.Enterprise;
            var networkInfo = AddressUtility.GetNetworkInfo(networkType);
            var paymentEncoded = HashUtility.Blake2b224(payment.Key);

            //get prefix
            var prefix = $"{AddressUtility.GetPrefixHeader(addressType)}{AddressUtility.GetPrefixTail(networkType)}";

            //get header
            var header = AddressUtility.GetHeader(networkInfo, addressType);
            
            //get body
            byte[] addressArray = new byte[1 + paymentEncoded.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(paymentEncoded, 0, addressArray, 1, paymentEncoded.Length);

            return new Address(prefix, addressArray);
        }

        [Obsolete("See AddressUtility.GetEnterpriseScriptAddress")]
        public Address GetEnterpriseScriptAddress<T>(T paymentPolicy, NetworkType networkType)
        {
            var addressType = AddressType.EnterpriseScript;
            var networkInfo = AddressUtility.GetNetworkInfo(networkType);
            
            Type paymentPolicyType = typeof(T);
            byte[] policyId = paymentPolicyType.Name switch
            {
                nameof(NativeScript) => ((NativeScript)Convert.ChangeType(paymentPolicy, typeof(NativeScript))).GetPolicyId(),
                nameof(ScriptAny) => ((ScriptAny)Convert.ChangeType(paymentPolicy, typeof(ScriptAny))).GetPolicyId(),
                nameof(ScriptAll) => ((ScriptAll)Convert.ChangeType(paymentPolicy, typeof(ScriptAll))).GetPolicyId(),
                nameof(ScriptNofK) => ((ScriptNofK)Convert.ChangeType(paymentPolicy, typeof(ScriptNofK))).GetPolicyId(),
                _ => throw new Exception("Unknown native script type for payment script")
            };

            //get prefix
            var prefix = $"{AddressUtility.GetPrefixHeader(addressType)}{AddressUtility.GetPrefixTail(networkType)}";

            //get header
            var header = AddressUtility.GetHeader(networkInfo, addressType);
            
            //get body
            byte[] addressArray = new byte[1 + policyId.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(policyId, 0, addressArray, 1, policyId.Length);

            return new Address(prefix, addressArray);
        }

        [Obsolete("See AddressUtility.GetBaseScriptAddress")]
        public Address GetBaseScriptAddress<T, K>(T paymentPolicy, K stakePolicy, NetworkType networkType)
        {
            var addressType = AddressType.BaseScript;
            var networkInfo = AddressUtility.GetNetworkInfo(networkType);
            
            Type paymentPolicyType = typeof(T);
            byte[] paymentPolicyId = paymentPolicyType.Name switch
            {
                nameof(NativeScript) => ((NativeScript)Convert.ChangeType(paymentPolicy, typeof(NativeScript))).GetPolicyId(),
                nameof(ScriptAny) => ((ScriptAny)Convert.ChangeType(paymentPolicy, typeof(ScriptAny))).GetPolicyId(),
                nameof(ScriptAll) => ((ScriptAll)Convert.ChangeType(paymentPolicy, typeof(ScriptAll))).GetPolicyId(),
                nameof(ScriptNofK) => ((ScriptNofK)Convert.ChangeType(paymentPolicy, typeof(ScriptNofK))).GetPolicyId(),
                _ => throw new Exception("Unknown native script type for payment script")
            };
            
            Type stakePolicyType = typeof(T);
            byte[] stakePolicyId = stakePolicyType.Name switch
            {
                nameof(NativeScript) => ((NativeScript)Convert.ChangeType(stakePolicy, typeof(NativeScript))).GetPolicyId(),
                nameof(ScriptAny) => ((ScriptAny)Convert.ChangeType(stakePolicy, typeof(ScriptAny))).GetPolicyId(),
                nameof(ScriptAll) => ((ScriptAll)Convert.ChangeType(stakePolicy, typeof(ScriptAll))).GetPolicyId(),
                nameof(ScriptNofK) => ((ScriptNofK)Convert.ChangeType(stakePolicy, typeof(ScriptNofK))).GetPolicyId(),
                _ => throw new Exception("Unknown native script type for stake script")
            };

            //get prefix
            var prefix = $"{AddressUtility.GetPrefixHeader(addressType)}{AddressUtility.GetPrefixTail(networkType)}";

            //get header
            var header = AddressUtility.GetHeader(networkInfo, addressType);
            
            //get body
            byte[] addressArray = new byte[1 + paymentPolicyId.Length + stakePolicyId.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(paymentPolicyId, 0, addressArray, 1, paymentPolicyId.Length);
            Buffer.BlockCopy(stakePolicyId, 0, addressArray, paymentPolicyId.Length + 1, stakePolicyId.Length);

            return new Address(prefix, addressArray);
        }

        [Obsolete("Use address.ExtractRewardAddress()")]
        public Address ExtractRewardAddress(Address basePaymentAddress)
        {
            if (basePaymentAddress.AddressType != AddressType.Base)
                throw new ArgumentException($"{nameof(basePaymentAddress)}:{basePaymentAddress} is not a base address", nameof(basePaymentAddress));

            // The stake key digest is the second half of a base address's bytes (pre-bech32)
            // and same value as the blake2b-224 hash digest of the stake key (blake2b-224=224bits=28bytes)
            const int stakeKeyDigestByteLength = 28;
            byte[] rewardAddressBytes = new byte[1 + stakeKeyDigestByteLength];
            var rewardAddressPrefix = $"{AddressUtility.GetPrefixHeader(AddressType.Reward)}{AddressUtility.GetPrefixTail(basePaymentAddress.NetworkType)}";
            var rewardAddressHeader = AddressUtility.GetHeader(AddressUtility.GetNetworkInfo(basePaymentAddress.NetworkType), AddressType.Reward);
            rewardAddressBytes[0] = rewardAddressHeader;
            // Extract stake key hash from baseAddressBytes 
            Buffer.BlockCopy(basePaymentAddress.GetBytes(), 29, rewardAddressBytes, 1, stakeKeyDigestByteLength);

            return new Address(rewardAddressPrefix, rewardAddressBytes);
        }
    }
}
