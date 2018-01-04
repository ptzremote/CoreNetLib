using System;

namespace CoreNetLib.NetClientLib
{
    public class DisconnectEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }
}
