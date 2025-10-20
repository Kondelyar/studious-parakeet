using EchoTspServer;

namespace NetSdrClientApp
{
    public class Dummy
    {
        public void CreateServer()
        {
            var server = new EchoServer(5000); // ось тут залежність на інфраструктуру
        }
    }
}
