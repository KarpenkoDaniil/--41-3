using Client.NetWork;
using GameLoader;
using IputController;
using OutputWindow.Network;
using System.Net;
using System.Net.Sockets;

namespace NetWorkConnector
{
    public class ConectionManager
    {
        public EventHandler _eventSend;
        public EventHandler _eventReceive;
        public int PlayerToken => _server.PlayerToken;

        Socket _socketPlayer;
        Client.NetWork.Server _server;

        List<StateObject> stateObjects = new List<StateObject>();

        public ConectionManager()
        {
            _socketPlayer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketPlayer.NoDelay = true;
            _socketPlayer.ReceiveBufferSize = 1024 * 24;
            _socketPlayer.SendBufferSize = 1024 * 24;
            _server = new Client.NetWork.Server(_socketPlayer);
        }

        //Соеденение с сервером
        public void SetConnection(IPEndPoint hostEndPoint)
        {
            _socketPlayer.Connect(hostEndPoint);

            Thread thread = new Thread(Player);
            thread.Start();
        }

        private void SendData()
        {
            while (true)
            {
                SendState();
                InputControlller.UpdateInput();
                Thread.Sleep(1000 / 60);
            }
        }

        private async void Player()
        {
            TimeDeltaHelper.Start();
            SingeltonEngine.GameClient.UpdateState(_server.ReciveData().GameStates);
            while (true)
            {
                SendState();
                List<StateObject> data;
                if (_socketPlayer.Available > 0)
                {
                    data = _server.ReciveData().GameStates;
                    SingeltonEngine.GameClient.UpdateState(data);
                }
                SingeltonEngine.GameClient.GameState();

                SingeltonEngine.Render.UpdateScreen();
                InputControlller.UpdateInput();
            }
        }

        public void AddState(StateObject stateObject)
        {
            bool inCollection = false;
            foreach (var state in stateObjects)
            {
                if (state.ObjectID == stateObject.ObjectID)
                {
                    inCollection = true;
                    break;
                }
            }

            if (!inCollection)
            {
                stateObjects.Add(stateObject);
            }
        }
        
        public object Monitor = new object();

        private void Timer()
        {
            DateTime now = DateTime.Now;
            lock (Monitor)
            {
                System.Threading.Monitor.Wait(Monitor);
            }

            string time = (DateTime.Now - now).ToString();
            string pathToFile = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\Log.txt";
            File.AppendAllText(pathToFile, "Input lag: " + time + ";");
        }

        private void SendState()
        {
            if (stateObjects.Count > 0)
            {
                //Thread thread = new Thread(Timer);
                //thread.Start();

                var curStatet = new List<StateObject>(stateObjects);
                _server.SendToServerCurrentState(new GameState(curStatet, _server.PlayerToken));
                stateObjects.Clear();
            }
        }
    }
}
