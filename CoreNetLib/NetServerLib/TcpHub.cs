using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CoreNetLib.ServerNetLib
{
    public class TcpHub : IHub
    {
        public event EventHandler<ReceivedDataEventArgs> OnDataReceived;
        public event EventHandler<string> OnEvent;

        TcpListener listener;
        const int defaultPort = 11000;
        internal DataCollector dataCollector;
        NetSettings netSettings;

        public TcpHub(int port = defaultPort)
        {
            dataCollector = new DataCollector();
            dataCollector.OnEvent += DataCollector_OnEvent;
            dataCollector.OnMessageReceived += DataCollector_OnMessageReceived;
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
            }
            catch (Exception ex)
            {
                OnEvent?.Invoke(null, ex.Message);
            }
        }

        private void DataCollector_OnEvent(object sender, string e)
        {
            OnEvent?.Invoke(sender, e);
        }

        public TcpHub(NetSettings netSettings)
            : this()
        {
            this.netSettings = netSettings;
        }

        private void DataCollector_OnMessageReceived(object sender, EventArgs e)
        {
            foreach (var message in dataCollector.GetMessageList())
            {
                OnDataReceived?.Invoke(this, new ReceivedDataEventArgs { Data = netSettings.deserializer.Invoke(message) });
            }
        }

        void IHub.Start()
        {
            listener.Start();

            AcceptManyTcpClientAsync();
        }

        void IHub.SendToAll(object message)
        {
            dataCollector.WriteToAllStream(netSettings.serializer(message));
        }

        async void AcceptManyTcpClientAsync()
        {
            while (true)
            {
                OnEvent?.Invoke(null, "Waiting for new connection...");
                var tcpClient = await listener.AcceptTcpClientAsync();

                new Task(() =>
                dataCollector.ReceiveBytesFromTcpClientAsync(tcpClient))
                .Start();
            }
        }
    }
}
