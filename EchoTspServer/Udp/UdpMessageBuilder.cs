namespace EchoTspServer.Udp
{
    /// <summary>
    /// Builds UDP messages with header and payload
    /// </summary>
    public class UdpMessageBuilder
    {
        private ushort _sequenceNumber = 0;
        private readonly Random _random;

        public UdpMessageBuilder()
        {
            _random = new Random();
        }

        public byte[] BuildMessage()
        {
            byte[] samples = new byte[1024];
            _random.NextBytes(samples);
            _sequenceNumber++;

            byte[] header = new byte[] { 0x04, 0x84 };
            byte[] sequence = BitConverter.GetBytes(_sequenceNumber);

            return header.Concat(sequence).Concat(samples).ToArray();
        }

        public ushort GetCurrentSequenceNumber() => _sequenceNumber;
    }
}