using GameLoader;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;

namespace Client.NetWork
{
    public class Server
    {
        Socket _serverSocket;
        int _playerToken;
        public int PlayerToken => _playerToken;
        public Server(Socket socket)
        {
            _serverSocket = socket;
        }

        public void SendToServerCurrentState(GameState stateObjects)
        {
            string mesage = JsonConvert.SerializeObject(stateObjects);
            byte[] dataSend = Encoding.UTF8.GetBytes(mesage);

            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(dataSend, 0, dataSend.Length);
                }
                dataSend = outputStream.ToArray();
            }

            // Сначала отправляем размер данных
            byte[] sizeBytes = BitConverter.GetBytes(dataSend.Length);
            _serverSocket.Send(sizeBytes);

            // Затем отправляем сами данные
            int totalSent = 0;
            while (totalSent < dataSend.Length)
            {
                int sent = _serverSocket.Send(dataSend, totalSent,
                    dataSend.Length - totalSent, SocketFlags.None);
                totalSent += sent;
            }
        }

        public GameState ReciveData()
        {
            var t = ReciveFromServer();
            byte[] buffer = new byte[4];
            _serverSocket.Receive(buffer);
            _playerToken = BitConverter.ToInt32(buffer, 0);

            if (t.Restart)
            {
                SingeltonEngine.Render.RestartRender();
            }

            lock (SingeltonEngine.ConectionManager.Monitor)
            {
                Monitor.Pulse(SingeltonEngine.ConectionManager.Monitor);
            }

            return t;
        }

        private GameState ReciveFromServer()
        {
            // Получаем размер данных
            byte[] sizeBuffer = new byte[4];
            int totalRead = 0;
            while (totalRead < 4)
            {
                int read = _serverSocket.Receive(sizeBuffer, totalRead, 4 - totalRead, SocketFlags.None);
                if (read == 0) break;
                totalRead += read;
            }

            int expectedSize = BitConverter.ToInt32(sizeBuffer, 0);

            // Получаем сами данные
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[8192]; // больший размер буфера для эффективности
                totalRead = 0;

                while (totalRead < expectedSize)
                {
                    int read = _serverSocket.Receive(buffer, 0,
                        Math.Min(buffer.Length, expectedSize - totalRead),
                        SocketFlags.None);

                    if (read == 0) break; // соединение закрыто

                    memoryStream.Write(buffer, 0, read);
                    totalRead += read;
                }

                using (var inputStream = new MemoryStream(memoryStream.ToArray()))
                using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                using (var outputStream = new MemoryStream())
                {
                    gzipStream.CopyTo(outputStream);
                    byte[] decompressedData = outputStream.ToArray();

                    // Преобразуем обратно в строку JSON
                    string json = Encoding.UTF8.GetString(decompressedData);

                    // Десериализуем обратно в объект
                    GameState state = JsonConvert.DeserializeObject<GameState>(json);

                    return state;
                }
            }
        }
    }
}

