using System;

namespace CoreNetLib
{
    public class DisconnectEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }
}
