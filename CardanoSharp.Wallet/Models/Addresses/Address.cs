using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Models.Addresses
{
    public class Address : IEqualityComparer<Address>, IEquatable<Address>, IEquatable<string>, IEquatable<byte[]>
    {
        private byte[] _bytes;
        private string _address;
        public AddressType AddressType { get; }
        public NetworkType NetworkType { get; }
        public Address()
        {
        }

        public Address(string prefix, byte[] address)
        {
            _bytes = address;
            _address = Bech32.Encode(address, prefix);
            
            Prefix = prefix;
            AddressType = GetAddressType();
            NetworkType = GetNetworkType();
        }

        public Address(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) 
                throw new ArgumentNullException(nameof(address));

            
            //if (!bech32.HasValidChars(address))
            //{
            //    throw new ArgumentException("Invalid characters", nameof(address));
            //}

            _address = address;
            try
            {
                _bytes = Bech32.Decode(_address, out byte witVer, out string prefix);
                Prefix = prefix;
                WitnessVersion = witVer;
                AddressType = GetAddressType();
                NetworkType = GetNetworkType();
            }
            catch
            {
                NetworkType = NetworkType.Unknown;
            }
        }

        /// <summary>
        /// Returns <see cref="AddressType"/> as defined in https://github.com/cardano-foundation/CIPs/blob/master/CIP-0019/CIP-0019.md
        /// </summary>
        /// <returns>
        ///     <para><see cref="AddressType.Base"/> if AddressType header is 0x01</para>
        ///     <para><see cref="AddressType.Enterprise"/> if AddressType header is 0x06</para>
        ///     <para>otherwise <see cref="AddressType.Base"/></para>
        /// </returns>
        /// <returns></returns>
        private AddressType GetAddressType()
        {
            return (_bytes[0] >> 4) switch
            {
                0x01 => AddressType.Base, //@TODO: derive all AddressTypes
                0x06 => AddressType.Enterprise,
                //0x07 => AddressType.SmartContract,
                _ => AddressType.Base,
            };
        }

        /// <summary>
        /// Returns <see cref="NetworkType"/> as defined in https://github.com/cardano-foundation/CIPs/blob/master/CIP-0019/CIP-0019.md
        /// </summary>
        /// <returns>
        ///     <para><see cref="NetworkType.Testnet"/> if Metwork header is 0x00</para>
        ///     <para><see cref="NetworkType.Mainnet"/> if Network header is 0x01</para>
        /// </returns>
        /// <exception cref="InvalidOperationException">If LSB is </exception>"
        private NetworkType GetNetworkType()
        {
            return (_bytes[0] & 0x0f) switch
            {
                0x00 => NetworkType.Testnet,
                0x01 => NetworkType.Mainnet,
                _ => NetworkType.Unknown,
            };
        }
        
        public string Prefix { get; set; }
        public byte WitnessVersion { get; set; }

        public bool Equals(string other)
        {
            return _address.Equals(other);
        }

        public bool Equals(byte[] other)
        {
            return _bytes.Equals(other);
        }

        public bool Equals(Address other)
        {
            if (other == null) return false;
            return _bytes.Equals(other.GetBytes());
        }

        public byte[] GetBytes()
        {
            return _bytes;
        }

        public override string ToString()
        {
            return _address;
        }

        public string ToStringHex()
        {
            return _bytes.ToStringHex();
        }

        public bool Equals(Address x, Address y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Address obj)
        {
            return obj.GetBytes().GetHashCode();
        }
    }
}
