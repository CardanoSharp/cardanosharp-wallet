using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    public interface ICoseMessage
    {
        Headers Headers { get; }
    }
}
