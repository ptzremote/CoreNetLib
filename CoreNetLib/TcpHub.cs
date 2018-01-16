using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CoreNetLib
{
    public class TcpHub : IHub
    {
        public event EventHandler<ReceivedDataEventArgs> OnDataReceived;

        TcpListener listener;
        const int defaultPort = 11000;
        internal MessageHub messageHub;
        NetSettings netSettings;

        ILogger Logger { get; } = CoreNetLogging.CreateLogger<TcpHub>();

        public TcpHub(int port = defaultPort)
        {
            if (netSettings == null)
                netSettings = new NetSettings();

            messageHub = new MessageHub();
            messageHub.OnMessageReceived += DataCollector_OnMessageReceived;
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        public TcpHub(NetSettings netSettings)
            : this()
        {
            this.netSettings = netSettings;
        }

        private void DataCollector_OnMessageReceived(object sender, EventArgs e)
        {
            foreach (var message in messageHub.GetMessageList())
            {
                OnDataReceived?.Invoke(this, new ReceivedDataEventArgs { Data = netSettings.deserializer.Invoke(message) });
            }
        }

        public void Start()
        {
            listener.Start();

            AcceptManyTcpClientAsync();
        }

        public void SendToAll(object message)
        {
            messageHub.SendMessageToAll(netSettings.serializer(message));
        }

        async void AcceptManyTcpClientAsync()
        {
            while (true)
            {
                Logger.LogInformation("Waiting for new connection...");
                var tcpClient = await listener.AcceptTcpClientAsync();

                new Task(() =>
                messageHub.ReceiveBytesFromTcpClientAsync(tcpClient))
                .Start();
            }
        }
    }
}
