using System;
using System.Collections.Generic;
using System.Text;

namespace CardanoSharp.Wallet.CIPs.CIP8.Models
{
    /// <summary>
    /// All Cose Messages have a header
    /// </summary>
    public interface ICoseMessage
    {
        Headers Headers { get; }
    }
}
