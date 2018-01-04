using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace CoreNetLib
{
    public class TcpClientInfo
    {
        public Guid ClientId { get; internal set; }
        public NetworkStream Stream { get; set; }
        public TcpClient TcpClient { get; set; }
        public Queue<byte[]> ReceivedMessage { get; set; }

        public TcpClientInfo(TcpClient tcpClient)
        {
            this.ClientId = Guid.NewGuid();
            this.TcpClient = tcpClient;
            ReceivedMessage = new Queue<byte[]>();

            Stream = tcpClient.GetStream();
        }
    }
}
