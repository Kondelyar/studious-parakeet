using System.Net;
using EchoTspServer.Handlers;
using EchoTspServer.Infrastructure;
using EchoTspServer.Server;
using EchoTspServer.Udp;

namespace EchoTspServer
{
    /// <summary>
    /// Entry point for the Echo Server application
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Setup dependencies
            var listener = new TcpListenerWrapper(IPAddress.Any, 5000);
            var clientHandler = new EchoClientHandler();
            var server = new EchoServer(listener, clientHandler);

            // Start the server
            _ = Task.Run(() => server.StartAsync());

            // Setup UDP sender
            string host = "127.0.0.1";
            int port = 60000;
            int intervalMilliseconds = 5000;

            using (var sender = new UdpTimedSender(host, port))
            {
                Console.WriteLine("Press any key to stop sending...");
                sender.StartSending(intervalMilliseconds);

                Console.WriteLine("Press 'q' to quit...");
                while (Console.ReadKey(intercept: true).Key != ConsoleKey.Q)
                {
                    // Wait until 'q' is pressed
                }

                sender.StopSending();
                server.Stop();
                Console.WriteLine("Sender stopped.");
            }
        }
    }
}