using System;
using System.Collections.Generic;

namespace CardanoSharp.Wallet.Models.Transactions
{
    //TODO Implement Relays in the future
    public partial class Relays
    {
        public static implicit operator Relays(HashSet<Relays> v)
        {
            throw new NotImplementedException();
        }
    }

    public enum RelayEnum
    {
        SingleHostAddr,
        SingleHostName,
        MultiHostName,
    }
}