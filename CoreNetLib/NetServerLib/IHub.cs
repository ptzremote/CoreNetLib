using System;

namespace CoreNetLib.ServerNetLib
{
    public interface IHub
    {
        event EventHandler<ReceivedDataEventArgs> OnDataReceived;
        event EventHandler<string> OnEvent;
        void Start();

        void SendToAll(object data);
    }
}
