using Newtonsoft.Json;
using OutputWindow.Game;
using OutputWindow.Network;
using Server.GameLogic.InputController;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;

namespace Server.Network
{
    public class Player
    {
        public PlayerInputController InputController;
        private Queue<GameState> _gameStateQueue = new Queue<GameState>();
        Socket _playerSocket;
        EventHandler _reciveHandler;
        int playerToken;

        public int PlayerToken => playerToken;

        public Player(Socket socket)
        {
            _playerSocket = socket;
            playerToken = GetHashCode();
            InputController = new PlayerInputController(playerToken);
            Thread thread = new Thread(ReciveData);
            thread.Start();

            if (LoggerOn)
            {
                Thread logger = new Thread(Logger);
                logger.Start();
            }
        }

        public void EndUpdate()
        {
            InputController.UpdateInput();
        }

        public void SendToken()
        {
            byte[] data = BitConverter.GetBytes(playerToken);
            _playerSocket.Send(data);
        }

        public void SendData(List<StateObject> mesages, bool restart = false)
        {
            var gameState = new GameState(mesages, playerToken);
            gameState.Restart = restart;
            string mesage = JsonConvert.SerializeObject(gameState);
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
            _playerSocket.Send(sizeBytes);

            // Затем отправляем сами данные
            int totalSent = 0;
            while (totalSent < dataSend.Length)
            {
                int sent = _playerSocket.Send(dataSend, totalSent,
                    dataSend.Length - totalSent, SocketFlags.None);
                totalSent += sent;
            }
            //Console.WriteLine("Sended data bytes " + dataSend.Length + "\n Sended count object's " + mesages.Count + "\n");
        }

        float _time = 1f;

        float _timer_1 = 0f;
        float _timer_2 = 0f;

        int countRecivedMes = 0;
        int countRecivedAndUdpated = 0;

        private Queue<string> LoggQuery = new Queue<string>();
        private bool LoggerOn = false;

        private void Logger()
        {
            string pathToFile = AppDomain.CurrentDomain.BaseDirectory + "Logger\\Logger.txt";
            while (true)
            {
                if (LoggQuery.Count > 0)
                {
                    lock (LoggQuery)
                    {
                        File.AppendAllText(pathToFile, LoggQuery.Dequeue());
                    }
                }
            }
        }

        //Прием данных от пользователя
        private void ReciveData()
        {
            while (true)
            {
                if (_playerSocket.Available > 0)
                {
                    // Получаем размер данных
                    byte[] sizeBuffer = new byte[4];
                    int totalRead = 0;
                    while (totalRead < 4)
                    {
                        int read = _playerSocket.Receive(sizeBuffer, totalRead, 4 - totalRead, SocketFlags.None);
                        if (read == 0) break;
                        totalRead += read;
                    }

                    int expectedSize = BitConverter.ToInt32(sizeBuffer, 0);

                    // Получаем сами данные
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[expectedSize]; // больший размер буфера для эффективности
                        totalRead = 0;

                        while (totalRead < expectedSize)
                        {
                            int read = _playerSocket.Receive(buffer, 0,
                                Math.Min(buffer.Length, expectedSize - totalRead),
                                SocketFlags.None);

                            if (read == 0) break; // соединение закрыто   

                            memoryStream.Write(buffer, 0, read);
                            totalRead += read;
                        }

                        if (LoggerOn)
                        {
                            lock (LoggQuery)
                            {
                                string str = "Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff") + ", Byte's: " + expectedSize + ";\n";
                                LoggQuery.Enqueue(str);
                            }
                        }

                        GameState gameStatet;

                        using (var inputStream = new MemoryStream(memoryStream.ToArray()))
                        using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                        using (var outputStream = new MemoryStream())
                        {
                            gzipStream.CopyTo(outputStream);
                            byte[] decompressedData = outputStream.ToArray();

                            // Преобразуем обратно в строку JSON
                            string json = Encoding.UTF8.GetString(decompressedData);

                            // Десериализуем обратно в объект
                            gameStatet = JsonConvert.DeserializeObject<GameState>(json);
                        }

                        if (_timer_1 > _time)
                        {
                            _timer_1 = 0;
                            countRecivedMes++;
                            Console.WriteLine("Count recived mesages from user per second: " + countRecivedMes);
                            countRecivedMes = 0;
                        }
                        else
                        {
                            countRecivedMes++;
                            _timer_1 =  _timer_1 + TimeDeltaHelper.DeltaT;
                        }

                        foreach (var item in gameStatet.PushKeys)
                        {
                            InputController.KeyValuePairsPush[item.Key] = item.Value;
                        }

                        foreach (var item in gameStatet.DownKeys)
                        {
                            InputController.KeyValuePairsDown[item.Key] = item.Value;
                        }

                        foreach (var item in gameStatet.UpKeys)
                        {
                            InputController.KeyValuePairsUp[item.Key] = item.Value;
                        }

                        gameStatet.SatePlayer(this);

                        lock (_gameStateQueue)
                        {
                            if (_gameStateQueue.Count < _count)
                            {
                                _gameStateQueue.Enqueue(gameStatet);
                                countRecivedAndUdpated++;
                            }
                            else
                            {
                                if (_gameStateQueue.Count < _count * 4)
                                {
                                    if (_timerToRecive > _timeToWait)
                                    {
                                        _gameStateQueue.Enqueue(gameStatet);
                                        _timerToRecive = 0;
                                        countRecivedAndUdpated++;
                                    }
                                    else
                                    {
                                        _timerToRecive = _timerToRecive + TimeDeltaHelper.DeltaT;
                                    }
                                }
                            }
                        }

                        if (_timer_2 > _time)
                        {
                            Console.WriteLine("Count updated mesages from user per second: " + countRecivedAndUdpated);
                            _timer_2 = 0;
                            countRecivedAndUdpated = 0;
                        }
                        else
                        {
                            _timer_2 = _timer_2 + TimeDeltaHelper.DeltaT;
                        }
                        //Console.WriteLine("Recive data bytes " + expectedSize + "\n Recive count object's " + gameStatet.GameStates.Count + "\n");
                    }
                }
            }
        }

        private int _count = 30;
        private float _timerToRecive = 0f;
        private int _timeToWait => _count / 1000;

        public GameState GetGameStatet()
        {
            GameState gameState = null;

            if (_gameStateQueue.Count > 0)
            {
                gameState = _gameStateQueue.Dequeue();
                //Console.WriteLine("--------------------------");
                //Console.WriteLine("Player token " + playerToken);
                //Console.WriteLine(_gameStateQueue.Count);
                //Console.WriteLine("--------------------------");
            }

            return gameState;
        }
    }
}
