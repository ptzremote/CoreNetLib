using CoreNetLib;
using SharedLib;
using System;

namespace ChatClient
{
    class Program
    {
        static NetClient client;
        static void Main(string[] args)
        {
            client = new NetClient();
            client.OnDataReceived += Client_OnDataReceived;
            client.Connect("127.0.0.1", 11000);

            Console.Write("Enter username: ");
            var author = Console.ReadLine();
            
            while (true)
            {
                string input = Console.ReadLine();
                client.SendAsync(new Message { Author = author, Text = input});
            }
        }

        private static void Client_OnDataReceived(object sender, ReceivedDataEventArgs e)
        {
            var message = e.Data as Message;
            Console.WriteLine($"{message.Author} say: {message.Text}");
        }
    }
}
