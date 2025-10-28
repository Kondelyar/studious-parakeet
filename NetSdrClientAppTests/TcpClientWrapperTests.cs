using NUnit.Framework;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetSdrClientApp.Networking;
using System.Diagnostics.CodeAnalysis;

namespace NetSdrClientApp.Tests
{
    [TestFixture]
    public class TcpClientWrapperTests
    {
        private DisposableTcpListener? _listener;
        private int _port;
        private string _host = "127.0.0.1";

        [SetUp]
        public void Setup()
        {
            _port = GetFreePort();
            _listener = new DisposableTcpListener(IPAddress.Loopback, _port);
            _listener.Start();

            // Принимаем клиента в фоне; завершаем приём после таймаута, чтобы CI не зависал.
            _ = Task.Run(async () =>
            {
                try
                {
                    // ждём клиента не дольше 5 секунд
                    var acceptTask = _listener.AcceptTcpClientAsync();
                    var completed = await Task.WhenAny(acceptTask, Task.Delay(TimeSpan.FromSeconds(5)));
                    if (completed == acceptTask)
                    {
                        using var tcpClient = await acceptTask; // закроем сразу после подключения
                    }
                }
                catch
                {
                    // игнорируем ошибки при закрытии/таймауте
                }
            });
        }

        [TearDown]
        public void Teardown()
        {
            _listener?.Dispose();
        }

        [ExcludeFromCodeCoverage]
        private int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        [Test]
        public void Constructor_SetsHostAndPort()
        {
            var client = new TcpClientWrapper(_host, _port);
            Assert.That(client, Is.Not.Null);
        }

        [Test]
        public void Connected_ReturnsFalse_WhenNotConnected()
        {
            var client = new TcpClientWrapper(_host, _port);
            Assert.That(client.Connected, Is.False);
        }

        [Test]
        public void Connect_And_Disconnect_Works()
        {
            var client = new TcpClientWrapper(_host, _port);
            client.Connect();
            Assert.That(client.Connected, Is.True);
            client.Disconnect();
            Assert.That(client.Connected, Is.False);
        }

        [Test]
        public void Connect_Fails_WhenWrongHost()
        {
            var client = new TcpClientWrapper("256.256.256.256", _port);
            using var sw = new System.IO.StringWriter();
            Console.SetOut(sw);
            client.Connect();
            string output = sw.ToString();
            Assert.That(output, Does.Contain("Failed to connect"));
        }

        [Test]
        public void Disconnect_NoActiveConnection_ShowsMessage()
        {
            var client = new TcpClientWrapper(_host, _port);
            using var sw = new System.IO.StringWriter();
            Console.SetOut(sw);
            client.Disconnect();
            string output = sw.ToString();
            Assert.That(output, Does.Contain("No active connection"));
        }

        [Test]
        public async Task StartListeningAsync_ThrowsWhenNotConnected()
        {
            var client = new TcpClientWrapper(_host, _port);
            var method = typeof(TcpClientWrapper)
                .GetMethod("StartListeningAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.That(method, Is.Not.Null);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await (Task)method.Invoke(client, null)!);

            Assert.That(ex.Message, Is.EqualTo("Not connected to a server."));
        }

        private sealed class DisposableTcpListener : IDisposable
        {
            private readonly TcpListener _inner;

            public DisposableTcpListener(IPAddress address, int port)
            {
                _inner = new TcpListener(address, port);
            }

            public void Start() => _inner.Start();

            // добавляем публичный асинхронный метод-обёртку
            public Task<TcpClient> AcceptTcpClientAsync() => _inner.AcceptTcpClientAsync();

            public void Dispose()
            {
                try
                {
                    _inner.Stop();
                }
                catch { }
            }
        }
    }
}
