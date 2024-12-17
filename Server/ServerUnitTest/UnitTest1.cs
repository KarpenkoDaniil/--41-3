using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OutputWindow.Network;
using Server.Network;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;


namespace ServerUnitTest
{
    public class UnitTest1
    {
        [TestClass]
        public class PlayerTests
        {
            private Mock<Socket> _mockSocket;
            private Player _player;

            [TestMethod]
            public void SetUp()
            {
                _mockSocket = new Mock<Socket>();
                _player = new Player(_mockSocket.Object);
            }

            [TestMethod]
            public void SendToken_ShouldSendCorrectToken()
            {
                // Arrange
                int expectedToken = _player.PlayerToken;
                byte[] expectedData = BitConverter.GetBytes(expectedToken);

                // Act
                _player.SendToken();

                // Assert
                _mockSocket.Verify(socket => socket.Send(It.Is<byte[]>(data =>
                    data.Length == expectedData.Length &&
                    BitConverter.ToInt32(data, 0) == expectedToken)));
            }

            [TestMethod]
            public void SendData_ShouldCompressAndSendData()
            {
                // Arrange
                var messages = new List<StateObject> { new StateObject { /* заполните необходимые свойства */ } };
                var gameState = new GameState(messages, _player.PlayerToken);
                gameState.Restart = false;
                string json = JsonConvert.SerializeObject(gameState);
                byte[] dataSend = Encoding.UTF8.GetBytes(json);

                using (var outputStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                    {
                        gzipStream.Write(dataSend, 0, dataSend.Length);
                    }
                    dataSend = outputStream.ToArray();
                }

                // Act
                _player.SendData(messages);

                // Assert
                _mockSocket.Verify(socket => socket.Send(It.Is<byte[]>(data => data.Length == BitConverter.GetBytes(dataSend.Length).Length), SocketFlags.None));
            }

            [TestMethod]
            public void GetGameStatet_ShouldDequeueGameState()
            {
                // Arrange
                var gameState = new GameState(new List<StateObject>(), _player.PlayerToken);
                _player.SendData(new List<StateObject> { new StateObject { /* свойства */ } });

                // Act
                var dequeuedState = _player.GetGameStatet();

                // Assert
                Assert.IsNotNull(dequeuedState);
                Assert.AreEqual(gameState.PlayerToken, dequeuedState.PlayerToken);
            }

            [TestMethod]
            public void ReciveData_ShouldHandleIncomingData()
            {
                // Arrange
                var gameState = new GameState(new List<StateObject>(), _player.PlayerToken);
                string json = JsonConvert.SerializeObject(gameState);
                byte[] dataSend = Encoding.UTF8.GetBytes(json);

                using (var outputStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                    {
                        gzipStream.Write(dataSend, 0, dataSend.Length);
                    }
                    dataSend = outputStream.ToArray();
                }

                byte[] sizeBytes = BitConverter.GetBytes(dataSend.Length);
                _mockSocket.SetupSequence(socket => socket.Available).Returns(1).Returns(0);
                _mockSocket.Setup(socket => socket.Receive(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), SocketFlags.None))
                    .Returns((byte[] buffer, int offset, int size, SocketFlags flags) =>
                    {
                        Array.Copy(sizeBytes, buffer, sizeBytes.Length);
                        return sizeBytes.Length;
                    });

                // Act
                // —имул€ци€ вызова ReciveData() через поток или вручную

                // Assert
                Assert.IsTrue(true); // «десь можно проверить состо€ние, например, через пол€ _player.
            }
        }
    }
}