using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CoreNetLib
{
    internal class MessageHub
    {
        internal const int BUFFER_SIZE = 65536;
        IClientInfoStorage clientInfoStorage;

        public event EventHandler<EventArgs> OnMessageReceived;
        public event EventHandler<string> OnEvent;

        public MessageHub()
        {
            clientInfoStorage = new TcpClientInfoStorage();
        }

        internal MessageHub(IClientInfoStorage cis)
        {
            clientInfoStorage = cis;
        }
        internal List<TcpClientInfo> GetClientList()
        {
            return new List<TcpClientInfo>(clientInfoStorage);
        }
        
        internal void SendMessageToAll(byte[] message)
        {
            var clientList = 
                new ReadOnlyCollection<TcpClientInfo>((clientInfoStorage as TcpClientInfoStorage)
                .ToList());

            Parallel.ForEach(clientList, client => WriteToAll(client));

            void WriteToAll(TcpClientInfo clientInfo)
            {
                try
                {
                    var bytes = PacketProtocol.WrapMessage(message);
                    clientInfo.Stream.WriteAsync(bytes, 0, bytes.Length);

                    OnEvent?.Invoke(null, $"{bytes.Length } bytes sent to {clientInfo.ClientId}.");
                }
                catch (Exception ex)
                {
                    OnEvent?.Invoke(null, ex.Message);
                    clientInfoStorage.RemoveInfo(clientInfo);
                }
            }
        }

        internal async void ReceiveBytesFromTcpClientAsync(TcpClient tcpClient)
        {
            var clientInfo = new TcpClientInfo(tcpClient);
            clientInfoStorage.StoreInfo(clientInfo);

            OnEvent?.Invoke(null, $"New connection accepted. Id: {clientInfo.ClientId}, " +
                $"Endpoint: {clientInfo.TcpClient.Client.RemoteEndPoint}");
            OnEvent?.Invoke(null, $"Curent client count: {clientInfoStorage.Count()}");

            PacketProtocol pp = new PacketProtocol(500000)
            {
                MessageArrived = (message) =>
                {
                    clientInfo.ReceivedMessage.Enqueue(message);
                    OnMessageReceived?.Invoke(this, EventArgs.Empty);
                }
            };

            try
            {
                while (true)
                {
                    var bytes = await GetNextBytePortionAsync(clientInfo.Stream);
                    pp.ByteReceived(bytes);
                }
            }
            catch (ClientDisconnectedException)
            {
                OnEvent?.Invoke(null, $"Client {clientInfo.ClientId} disconnected.");
            }
            catch (Exception ex)
            {
                OnEvent?.Invoke(null, ex.Message);
            }
            finally
            {
                clientInfoStorage.RemoveInfo(clientInfo);

                OnEvent?.Invoke(null, $"Curent client count: {clientInfoStorage.Count()}");
            }
        }
        internal List<byte[]> GetMessageList()
        {
            List<byte[]> messageList = new List<byte[]>();
            foreach (var client in GetClientList().Where(c => c.ReceivedMessage.Count > 0))
            {
                messageList.Add(client.ReceivedMessage.Dequeue());
            }
            return messageList;
        }
        internal async Task<byte[]> GetNextBytePortionAsync(NetworkStream stream)
        {
            byte[] receiveBuffer = new byte[BUFFER_SIZE];
            int readBytes = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);

            if (readBytes == 0)
                throw new ClientDisconnectedException();

            receiveBuffer = CheckReceiveBufferSize(receiveBuffer, readBytes);

            return receiveBuffer;
        }

        internal byte[] CheckReceiveBufferSize(byte[] receiveBuffer, int readBytes)
        {
            if (receiveBuffer.Length > readBytes)
            {
                byte[] smallerReceiveBuffer = new byte[readBytes];
                Array.Copy(receiveBuffer, smallerReceiveBuffer, readBytes);
                receiveBuffer = smallerReceiveBuffer;
            }
            return receiveBuffer;
        }
    }
}
