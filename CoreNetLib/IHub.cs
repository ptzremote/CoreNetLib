using System;

namespace CoreNetLib
{
    public interface IHub
    {
        event EventHandler<ReceivedDataEventArgs> OnDataReceived;
        void Start();

        void SendToAll(object data);
    }
}
