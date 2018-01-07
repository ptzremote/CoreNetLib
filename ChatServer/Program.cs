using CoreNetLib;
using SharedLib;
using System;

namespace ChatServer
{
    class Program
    {
        static TcpHub hub;
        static void Main(string[] args)
        {
            hub = new TcpHub();
            hub.OnDataReceived += Hub_OnDataReceived;
            hub.Start();

            Console.ReadKey();
        }

        private static void Hub_OnDataReceived(object sender, ReceivedDataEventArgs e)
        {
            var message = e.Data as Message;
            hub.SendToAll(message);
        }
    }
}
