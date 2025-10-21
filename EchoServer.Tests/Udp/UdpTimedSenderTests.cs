using EchoTspServer.Udp;
using Moq;
using NUnit.Framework;

namespace EchoTspServer.Tests.Udp
{
    [TestFixture]
    public class UdpTimedSenderTests
    {
        private Mock<UdpMessageBuilder> _mockBuilder;
        private UdpTimedSender _sender;

        [SetUp]
        public void SetUp()
        {
            _mockBuilder = new Mock<UdpMessageBuilder>();
        }

        [TearDown]
        public void TearDown()
        {
            _sender?.Dispose();
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenHostIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new UdpTimedSender(null, 5000, _mockBuilder.Object));
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenMessageBuilderIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new UdpTimedSender("127.0.0.1", 5000, null));
        }

        [Test]
        public void Constructor_WithHostAndPort_CreatesInstance()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
                _sender = new UdpTimedSender("127.0.0.1", 5000));
        }

        [Test]
        public void StartSending_ThrowsException_WhenAlreadyStarted()
        {
            // Arrange
            _sender = new UdpTimedSender("127.0.0.1", 5000, _mockBuilder.Object);
            _sender.StartSending(1000);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                _sender.StartSending(1000));
        }

        [Test]
        public void StopSending_CanBeCalledMultipleTimes()
        {
            // Arrange
            _sender = new UdpTimedSender("127.0.0.1", 5000, _mockBuilder.Object);

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _sender.StopSending();
                _sender.StopSending();
            });
        }

        [Test]
        public void Dispose_StopsSending()
        {
            // Arrange
            _sender = new UdpTimedSender("127.0.0.1", 5000, _mockBuilder.Object);
            _sender.StartSending(1000);

            // Act & Assert
            Assert.DoesNotThrow(() => _sender.Dispose());
        }
    }
}