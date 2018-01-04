using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CoreNetLib
{
    internal interface IClientInfoStorage : IEnumerable<TcpClientInfo>
    {
        void StoreInfo(TcpClientInfo tci);
        int GetClientCount();
        Guid GetGuidByClientStream(NetworkStream stream);
        void RemoveInfo(TcpClientInfo tci);
    }
}
