using System;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Utilities;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class AddressExtensions
    {
        public static Address ToAddress(this string addr)
        {
            return new Address(addr);
        }
        
        public static bool HasValidNetwork(this Address address)
        {
            return address.NetworkType != NetworkType.Unknown;
        }

        public static byte[] GetPublicKeyHash(this Address address)
        {
            byte[] pkh = new byte[28];
            Buffer.BlockCopy(address.GetBytes(), 1, pkh, 0, pkh.Length);
            return pkh;
        }

        public static byte[]? GetStakeKeyHash(this Address address)
        {
            if (address.AddressType != AddressType.Base
                && address.AddressType != AddressType.BaseScript)
                throw new Exception("Address does not contain a stake key");
            
            byte[] pkh = new byte[28];
            Buffer.BlockCopy(address.GetBytes(), 29, pkh, 0, pkh.Length);
            return pkh;
        }
        
        public static Address GetStakeAddress(this Address address)
        {
            if (address.AddressType != AddressType.Base)
                throw new ArgumentException($"{nameof(address)}:{address} is not a base address", nameof(address));

            // The stake key digest is the second half of a base address's bytes (pre-bech32)
            // and same value as the blake2b-224 hash digest of the stake key (blake2b-224=224bits=28bytes)
            const int stakeKeyDigestByteLength = 28;
            byte[] rewardAddressBytes = new byte[1 + stakeKeyDigestByteLength];
            var rewardAddressPrefix = $"{AddressUtility.GetPrefixHeader(AddressType.Reward)}{AddressUtility.GetPrefixTail(address.NetworkType)}";
            var rewardAddressHeader = AddressUtility.GetHeader(AddressUtility.GetNetworkInfo(address.NetworkType), AddressType.Reward);
            rewardAddressBytes[0] = rewardAddressHeader;
            // Extract stake key hash from baseAddressBytes 
            Buffer.BlockCopy(address.GetBytes(), 29, rewardAddressBytes, 1, stakeKeyDigestByteLength);

            return new Address(rewardAddressPrefix, rewardAddressBytes);
        } 
    }
}
