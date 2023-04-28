using System;
using System.Collections.Generic;
using System.Linq;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Transactions.TransactionWitness.PlutusScripts;
using PeterO.Cbor2;

namespace CardanoSharp.Wallet.Utilities
{
    public static class SlotUtility
    {
        public static SlotNetworkConfig Mainnet { get; set; } = new SlotNetworkConfig(1596059091000, 4492800, 1000); // Starting at Shelly Era
        public static SlotNetworkConfig Preprod { get; set; } = new SlotNetworkConfig(1654041600000 + 1728000000, 86400, 1000); // Starting at Shelly Era
        public static SlotNetworkConfig Preview { get; set; } = new SlotNetworkConfig(1666656000000, 0, 1000); // Starting at Shelly Era

        public static SlotNetworkConfig GetSlotNetworkConfig(NetworkType networkType)
        {
            if (networkType == NetworkType.Mainnet)
                return Mainnet;
            else if (networkType == NetworkType.Preprod)
                return Preprod;
            else if (networkType == NetworkType.Preview)
                return Preview;

            return new SlotNetworkConfig();
        }
    }
}