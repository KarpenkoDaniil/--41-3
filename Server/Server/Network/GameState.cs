using OutputWindow.Network;

namespace Server.Network
{
    public class GameState
    {
        public Dictionary<string, bool> PushKeys = new Dictionary<string, bool>();
        public Dictionary<string, bool> DownKeys = new Dictionary<string, bool>();
        public Dictionary<string, bool> UpKeys = new Dictionary<string, bool>();

        public List<StateObject> GameStates;

        private Player _player;
        public Player Player => _player;

        public int PlayerToken;
        public bool Restart = false;

        public GameState() { }

        public GameState(List<StateObject> gameStates, int playerToken)
        {
            GameStates = gameStates;
            PlayerToken = playerToken;
        }

        public void SatePlayer(Player player)
        {
            _player = player;
        }  
    }
}
