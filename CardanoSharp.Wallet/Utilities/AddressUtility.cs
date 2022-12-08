using System;
using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.NativeScripts;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;

namespace CardanoSharp.Wallet.Utilities
{
    public static class AddressUtility
    {
        public static NetworkInfo GetNetworkInfo(NetworkType type) =>
            type switch
            {
                NetworkType.Testnet => new NetworkInfo(0b0000, 1097911063),
                NetworkType.Preview => new NetworkInfo(0b0000, 2),
                NetworkType.Preprod => new NetworkInfo(0b0000, 1),
                NetworkType.Mainnet => new NetworkInfo(0b0001, 764824073),
                _ => throw new Exception("Unknown network type")
            };

        public static byte GetHeader(NetworkInfo networkInfo, AddressType addressType) =>
            addressType switch
            {
                AddressType.Base => (byte)(networkInfo.NetworkId & 0xF),
                AddressType.BaseScript => (byte)(0b0011_0000 | networkInfo.NetworkId & 0xF),
                AddressType.Enterprise => (byte)(0b0110_0000 | networkInfo.NetworkId & 0xF),
                AddressType.Reward => (byte)(0b1110_0000 | networkInfo.NetworkId & 0xF),
                AddressType.EnterpriseScript => (byte)(0b0111_0000 | networkInfo.NetworkId & 0xF),
                _ => throw new Exception("Unknown address type")
            };

        public static string GetPrefix(AddressType addressType, NetworkType networkType) =>
            $"{GetPrefixHeader(addressType)}{GetPrefixTail(networkType)}";

        public static string GetPrefixHeader(AddressType addressType) =>
            addressType switch
            {
                AddressType.Reward => "stake",
                AddressType.Base => "addr",
                AddressType.BaseScript => "addr",
                AddressType.Enterprise => "addr",
                AddressType.EnterpriseScript => "addr",
                _ => throw new Exception("Unknown address type")
            };

        public static string GetPrefixTail(NetworkType networkType) =>
            networkType switch
            {
                NetworkType.Testnet => "_test",
                NetworkType.Preview => "_test",
                NetworkType.Preprod => "_test",
                NetworkType.Mainnet => "",
                _ => throw new Exception("Unknown address type")
            };

        public static Address GetBaseAddress(
            PublicKey payment,
            PublicKey stake,
            NetworkType networkType
        )
        {
            var paymentEncoded = HashUtility.Blake2b224(payment.Key);
            var stakeEncoded = HashUtility.Blake2b224(stake.Key);
            return GetBaseAddress(paymentEncoded, stakeEncoded, networkType);
        }

        public static Address GetStakeAddress(PublicKey stake, NetworkType networkType)
        {
            var stakeEncoded = HashUtility.Blake2b224(stake.Key);
            return GetStakeAddress(stakeEncoded, networkType);
        }

        public static Address GetEnterpriseAddress(PublicKey payment, NetworkType networkType)
        {
            var paymentEncoded = HashUtility.Blake2b224(payment.Key);
            return GetEnterpriseAddress(paymentEncoded, networkType);
        }

        public static Address GetEnterpriseScriptAddress<T>(
            T paymentPolicy,
            NetworkType networkType
        )
        {
            var addressType = AddressType.EnterpriseScript;
            var networkInfo = GetNetworkInfo(networkType);

            Type paymentPolicyType = typeof(T);
            byte[] policyId = paymentPolicyType.Name switch
            {
                nameof(NativeScript)
                    => (
                        (NativeScript)Convert.ChangeType(paymentPolicy, typeof(NativeScript))
                    ).GetPolicyId(),
                nameof(ScriptAny)
                    => (
                        (ScriptAny)Convert.ChangeType(paymentPolicy, typeof(ScriptAny))
                    ).GetPolicyId(),
                nameof(ScriptAll)
                    => (
                        (ScriptAll)Convert.ChangeType(paymentPolicy, typeof(ScriptAll))
                    ).GetPolicyId(),
                nameof(ScriptNofK)
                    => (
                        (ScriptNofK)Convert.ChangeType(paymentPolicy, typeof(ScriptNofK))
                    ).GetPolicyId(),
                nameof(PlutusV1Script)
                    => (
                        (PlutusV1Script)Convert.ChangeType(paymentPolicy, typeof(PlutusV1Script))
                    ).GetPolicyId(),
                nameof(PlutusV2Script)
                    => (
                        (PlutusV2Script)Convert.ChangeType(paymentPolicy, typeof(PlutusV2Script))
                    ).GetPolicyId(),
                _ => throw new Exception("Unknown native script type for payment script")
            };

            //get prefix
            var prefix = $"{GetPrefixHeader(addressType)}{GetPrefixTail(networkType)}";

            //get header
            var header = GetHeader(networkInfo, addressType);

            //get body
            byte[] addressArray = new byte[1 + policyId.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(policyId, 0, addressArray, 1, policyId.Length);
            return new Address(prefix, addressArray);
        }

        public static Address GetBaseScriptAddress<T, K>(
            T paymentPolicy,
            K stakePolicy,
            NetworkType networkType
        )
        {
            var addressType = AddressType.BaseScript;
            var networkInfo = GetNetworkInfo(networkType);

            Type paymentPolicyType = typeof(T);
            byte[] paymentPolicyId = paymentPolicyType.Name switch
            {
                nameof(NativeScript)
                    => (
                        (NativeScript)Convert.ChangeType(paymentPolicy, typeof(NativeScript))
                    ).GetPolicyId(),
                nameof(ScriptAny)
                    => (
                        (ScriptAny)Convert.ChangeType(paymentPolicy, typeof(ScriptAny))
                    ).GetPolicyId(),
                nameof(ScriptAll)
                    => (
                        (ScriptAll)Convert.ChangeType(paymentPolicy, typeof(ScriptAll))
                    ).GetPolicyId(),
                nameof(ScriptNofK)
                    => (
                        (ScriptNofK)Convert.ChangeType(paymentPolicy, typeof(ScriptNofK))
                    ).GetPolicyId(),
                nameof(PlutusV1Script)
                    => (
                        (PlutusV1Script)Convert.ChangeType(paymentPolicy, typeof(PlutusV1Script))
                    ).GetPolicyId(),
                nameof(PlutusV2Script)
                    => (
                        (PlutusV2Script)Convert.ChangeType(paymentPolicy, typeof(PlutusV2Script))
                    ).GetPolicyId(),
                _ => throw new Exception("Unknown script type for payment script")
            };

            Type stakePolicyType = typeof(K);
            byte[] stakePolicyId = stakePolicyType.Name switch
            {
                nameof(NativeScript)
                    => (
                        (NativeScript)Convert.ChangeType(stakePolicy, typeof(NativeScript))
                    ).GetPolicyId(),
                nameof(ScriptAny)
                    => (
                        (ScriptAny)Convert.ChangeType(stakePolicy, typeof(ScriptAny))
                    ).GetPolicyId(),
                nameof(ScriptAll)
                    => (
                        (ScriptAll)Convert.ChangeType(stakePolicy, typeof(ScriptAll))
                    ).GetPolicyId(),
                nameof(ScriptNofK)
                    => (
                        (ScriptNofK)Convert.ChangeType(stakePolicy, typeof(ScriptNofK))
                    ).GetPolicyId(),
                nameof(PlutusV1Script)
                    => (
                        (PlutusV1Script)Convert.ChangeType(paymentPolicy, typeof(PlutusV1Script))
                    ).GetPolicyId(),
                nameof(PlutusV2Script)
                    => (
                        (PlutusV2Script)Convert.ChangeType(paymentPolicy, typeof(PlutusV2Script))
                    ).GetPolicyId(),
                _ => throw new Exception("Unknown script type for stake script")
            };

            //get prefix
            var prefix = $"{GetPrefixHeader(addressType)}{GetPrefixTail(networkType)}";

            //get header
            var header = GetHeader(networkInfo, addressType);

            //get body
            byte[] addressArray = new byte[1 + paymentPolicyId.Length + stakePolicyId.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(paymentPolicyId, 0, addressArray, 1, paymentPolicyId.Length);
            Buffer.BlockCopy(
                stakePolicyId,
                0,
                addressArray,
                paymentPolicyId.Length + 1,
                stakePolicyId.Length
            );
            return new Address(prefix, addressArray);
        }

        public static Address GetBaseScriptAddress<T>(
            T paymentPolicy,
            PublicKey stake,
            NetworkType networkType
        )
        {
            return GetBaseScriptAddress<T>(paymentPolicy, stake.Key, networkType);
        }

        public static Address GetBaseScriptAddress<T>(
            T paymentPolicy,
            byte[] stakeEncoded,
            NetworkType networkType
        )
        {
            var addressType = AddressType.BaseScript;
            var networkInfo = GetNetworkInfo(networkType);

            Type paymentPolicyType = typeof(T);
            byte[] paymentPolicyId = paymentPolicyType.Name switch
            {
                nameof(NativeScript)
                    => (
                        (NativeScript)Convert.ChangeType(paymentPolicy, typeof(NativeScript))
                    ).GetPolicyId(),
                nameof(ScriptAny)
                    => (
                        (ScriptAny)Convert.ChangeType(paymentPolicy, typeof(ScriptAny))
                    ).GetPolicyId(),
                nameof(ScriptAll)
                    => (
                        (ScriptAll)Convert.ChangeType(paymentPolicy, typeof(ScriptAll))
                    ).GetPolicyId(),
                nameof(ScriptNofK)
                    => (
                        (ScriptNofK)Convert.ChangeType(paymentPolicy, typeof(ScriptNofK))
                    ).GetPolicyId(),
                nameof(PlutusV1Script)
                    => (
                        (PlutusV1Script)Convert.ChangeType(paymentPolicy, typeof(PlutusV1Script))
                    ).GetPolicyId(),
                nameof(PlutusV2Script)
                    => (
                        (PlutusV2Script)Convert.ChangeType(paymentPolicy, typeof(PlutusV2Script))
                    ).GetPolicyId(),
                _ => throw new Exception("Unknown script type for payment script")
            };

            //get prefix
            var prefix = $"{GetPrefixHeader(addressType)}{GetPrefixTail(networkType)}";

            //get header
            var header = GetHeader(networkInfo, addressType);

            //get body
            byte[] addressArray = new byte[1 + paymentPolicyId.Length + stakeEncoded.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(paymentPolicyId, 0, addressArray, 1, paymentPolicyId.Length);
            Buffer.BlockCopy(
                stakeEncoded,
                0,
                addressArray,
                paymentPolicyId.Length + 1,
                stakeEncoded.Length
            );
            return new Address(prefix, addressArray);
        }

        public static Address GetBaseAddress(
            byte[] paymentEncoded,
            byte[] stakeEncoded,
            NetworkType networkType
        )
        {
            var addressType = AddressType.Base;
            var networkInfo = GetNetworkInfo(networkType);

            //get prefix
            var prefix = $"{GetPrefixHeader(addressType)}{GetPrefixTail(networkType)}";

            //get header
            var header = GetHeader(networkInfo, addressType);

            //get body
            byte[] addressArray = new byte[1 + paymentEncoded.Length + stakeEncoded.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(paymentEncoded, 0, addressArray, 1, paymentEncoded.Length);
            Buffer.BlockCopy(
                stakeEncoded,
                0,
                addressArray,
                paymentEncoded.Length + 1,
                stakeEncoded.Length
            );

            return new Address(prefix, addressArray);
        }

        public static Address GetStakeAddress(byte[] stakeEncoded, NetworkType networkType)
        {
            var addressType = AddressType.Reward;
            var networkInfo = GetNetworkInfo(networkType);

            //get prefix
            var prefix = $"{GetPrefixHeader(addressType)}{GetPrefixTail(networkType)}";

            //get header
            var header = GetHeader(networkInfo, addressType);

            //get body
            byte[] addressArray = new byte[1 + stakeEncoded.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(stakeEncoded, 0, addressArray, 1, stakeEncoded.Length);

            return new Address(prefix, addressArray);
        }

        public static Address GetEnterpriseAddress(byte[] paymentEncoded, NetworkType networkType)
        {
            var addressType = AddressType.Enterprise;
            var networkInfo = GetNetworkInfo(networkType);

            //get prefix
            var prefix = $"{GetPrefixHeader(addressType)}{GetPrefixTail(networkType)}";

            //get header
            var header = GetHeader(networkInfo, addressType);

            //get body
            byte[] addressArray = new byte[1 + paymentEncoded.Length];
            addressArray[0] = header;
            Buffer.BlockCopy(paymentEncoded, 0, addressArray, 1, paymentEncoded.Length);

            return new Address(prefix, addressArray);
        }
    }
}
