using System;
using System.Linq;

namespace EchoTspServer.Udp
{
    /// <summary>
    /// Builds UDP messages with header and payload
    /// </summary>
    public class UdpMessageBuilder
    {
        private ushort _sequenceNumber = 0;
        private readonly Random _random = new Random();

        public byte[] BuildMessage()
        {
            byte[] samples = new byte[1024];
            // генеруємо випадкові байти для тестових даних
            _random.NextBytes(samples);

            _sequenceNumber++;
            byte[] header = new byte[] { 0x04, 0x84 };
            byte[] sequence = BitConverter.GetBytes(_sequenceNumber);

            // перетворюємо у Big Endian для мережевого порядку
            if (BitConverter.IsLittleEndian)
                Array.Reverse(sequence);

            return header.Concat(sequence).Concat(samples).ToArray();
        }

        public ushort GetCurrentSequenceNumber() => _sequenceNumber;
    }
}
