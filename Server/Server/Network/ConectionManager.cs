using GameObjects;
using OutputWindow.Game;
using OutputWindow.Network;
using OutputWindow.Scripts.TankScripts;
using Server.Network;
using System.Net;
using System.Net.Sockets;

namespace NetWorkConnector
{
    public class ConectionManager
    {
        public EventHandler _eventSend;
        public EventHandler _eventReceive;

        private List<Player> _players = new List<Player>();
        private List<StateObject> stateObjects = new List<StateObject>();
        private List<PlayerTankScript> _playbelObjects = new List<PlayerTankScript>();

        Socket _socketHost;
        bool _isHost = false;

        public ConectionManager()
        {
            _socketHost = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketHost.NoDelay = true;
            _socketHost.SendBufferSize = 24 * 1024;
            _socketHost.ReceiveBufferSize = 24 * 1024;
        }

        public void SetHost(bool IHost, IPEndPoint endPoint)
        {
            _isHost = IHost;
            _socketHost.Bind(endPoint);
            Console.WriteLine("Server start on IP/PORT: " + _socketHost.LocalEndPoint);
            _socketHost.Listen(10);

            Thread thread = new Thread(Host);
            thread.Start();
        }

        public void SetPlaybelObjects()
        {
            foreach (GameObject gameObject in SingeltonEngine.GameEngine.GameObjects)
            {
                foreach (var item in gameObject.MonoBehaivours)
                {
                    if (item is PlayerTankScript playerTank)
                    {
                        _playbelObjects.Add(playerTank);
                        break;
                    }
                }
            }
        }

        public void Host()
        {
            while (true)
            {
                Socket playerSoc = _socketHost.Accept();
                var player = new Player(playerSoc);
                _players.Add(player);
                _playbelObjects[0].SetToken(player.PlayerToken);
                _playbelObjects.RemoveAt(0);
                SendDataOnStart(player);

                if (_players.Count > 2)
                {
                    break;
                }
            }
        }

        public void SendDataOnRestart()
        {
            _playbelObjects.Clear();
            foreach (var player in _players)
            {
                List<StateObject> stateObjects = new List<StateObject>();

                var gmOBJ = SingeltonEngine.GameEngine.GameObjects;

                foreach (var obj in gmOBJ)
                {
                    AddObjectToSend(obj, StateObject.TypeOfMesage.Create);
                }

                if (_playbelObjects.Count == 0)
                {
                    SetPlaybelObjects();
                }

                foreach (var obj in _playbelObjects)
                {
                    obj.SetToken(player.PlayerToken);
                    break;
                }

                if (_playbelObjects.Count > 0)
                {
                    _playbelObjects.Remove(_playbelObjects[0]);
                }
            }

            _playbelObjects.Clear();

            foreach (var player in _players)
            {
                player.SendData(stateObjects, true);
                player.SendToken();
            }
        }

        //ОТправка состояния на старте
        private void SendDataOnStart(Player player)
        {
            List<StateObject> stateObjects = new List<StateObject>();

            var gmOBJ = SingeltonEngine.GameEngine.GameObjects;

            foreach (var obj in gmOBJ)
            {
                stateObjects.Add(new StateObject(obj, StateObject.TypeOfMesage.Create));
            }

            player.SendData(stateObjects);
            player.SendToken();
        }

        public List<GameState> GetDataFromUsers()
        {
            List<GameState> gameStates = new List<GameState>();

            foreach (var player in _players)
            {
                var statet = player?.GetGameStatet();
                if (statet != null)
                {
                    gameStates.Add(statet);
                }
            }

            return gameStates;
        }

        //Отправка состояния
        public void SendStateFromServer()
        {

            if (_players.Count > 0 && stateObjects.Count > 0)
            {
                foreach (var player in _players)
                {
                    player.SendData(stateObjects);
                    player.SendToken();
                }

                lock (stateObjects)
                {
                    stateObjects.Clear();
                }
            }
        }

        public void AddObjectToSend(GameObject gameObject, StateObject.TypeOfMesage typeOfMesage)
        {
            bool canAdd = true;
            foreach (var stateObject in stateObjects)
            {
                if (stateObject.ObjectID == gameObject.GameObjectID)
                {
                    canAdd = false;
                }
            }

            if (canAdd)
            {
                stateObjects.Add(new StateObject(gameObject, typeOfMesage));
            }
        }

        public void EndOfState()
        {
            foreach (var player in _players)
            {
                player.EndUpdate();
            }
        }
    }
}
