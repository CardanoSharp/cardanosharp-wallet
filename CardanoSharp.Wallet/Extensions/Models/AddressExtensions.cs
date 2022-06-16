using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Enums;

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
    }
}
