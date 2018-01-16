using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CoreNetLib
{
    public class NetClient
    {
        TcpClient tcpClient;
        NetworkStream stream;
        MessageHub messageHub;
        NetSettings netSettings;
        ILogger Logger { get; } = CoreNetLogging.CreateLogger<TcpHub>();

        public event EventHandler<ReceivedDataEventArgs> OnDataReceived;
        public event EventHandler<DisconnectEventArgs> OnDisconnected;

        public bool Connected
        {
            get { return tcpClient.Connected; }
        }

        public NetClient()
            : this(new NetSettings())
        {

        }

        public NetClient(NetSettings netSettings)
        {
            this.netSettings = netSettings;
            tcpClient = new TcpClient();
            messageHub = new MessageHub();
            messageHub.OnMessageReceived += DataCollector_OnMessageReceived;
        }

        private void DataCollector_OnMessageReceived(object sender, EventArgs e)
        {
            foreach (var message in messageHub.GetMessageList())
            {
                OnDataReceived?.Invoke(this,
                new ReceivedDataEventArgs { Data = netSettings.deserializer.Invoke(message) });
            }
        }

        public void Connect(string hostname, int port)
        {
            try
            {
                tcpClient.Connect(hostname, port);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            stream = tcpClient.GetStream();

            new Task(() =>
            messageHub.ReceiveBytesFromTcpClientAsync(tcpClient))
            .Start();
        }
        public void Disconnect()
        {
            tcpClient.Client.Disconnect(false);
            tcpClient.Close();
        }
        public async void SendAsync(object data)
        {
            try
            {
                var bytes = PacketProtocol.WrapMessage(netSettings.serializer.Invoke(data));
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                OnDisconnected?.Invoke(null, new DisconnectEventArgs { Exception = ex });
            }
        }
    }
}
