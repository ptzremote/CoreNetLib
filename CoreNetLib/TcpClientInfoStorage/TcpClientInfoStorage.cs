using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CoreNetLib
{
    class TcpClientInfoStorage : IClientInfoStorage, IEnumerable<TcpClientInfo>
    {
        List<TcpClientInfo> clientInfos;

        public TcpClientInfoStorage()
        {
            clientInfos = new List<TcpClientInfo>();
        }

        public void StoreInfo(TcpClientInfo tci)
        {
            clientInfos.Add(tci);
        }

        public void RemoveInfo(TcpClientInfo tci)
        {
            clientInfos.Remove(tci);
        }

        public Guid GetGuidByClientStream(NetworkStream stream)
        {
            var tcpClientInfo = clientInfos.Where(ci => ci.Stream == stream).
                FirstOrDefault();
            if (tcpClientInfo == default(TcpClientInfo))
                return Guid.Empty;
            return tcpClientInfo.ClientId;
        }

        public int GetClientCount()
        {
            return clientInfos.Count;
        }

        public IEnumerator<TcpClientInfo> GetEnumerator()
        {
            return clientInfos.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return clientInfos.GetEnumerator();
        }
    }
}
