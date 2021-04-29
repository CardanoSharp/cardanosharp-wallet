using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.Common
{
    public class NetworkInfo
    {
        public int NetworkId { get; set; }
        public int ProtocolMagic { get; set; }

        public NetworkInfo(int networkId, int protocolMagic)
        {
            NetworkId = networkId;
            ProtocolMagic = protocolMagic;
        }
    }
}
