using CardanoSharp.Wallet.Models.Addresses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Extensions.Models
{
    public static class AddressExtensions
    {
        public static Address ToAddress(this string addr)
        {
            return new Address(addr);
        }
    }
}
