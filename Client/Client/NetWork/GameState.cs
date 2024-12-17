using IputController;
using OutputWindow.Network;

namespace Client.NetWork
{
    public class GameState
    {
        public Dictionary<string, bool> PushKeys = new Dictionary<string, bool>();
        public Dictionary<string, bool> DownKeys = new Dictionary<string, bool>();
        public Dictionary<string, bool> UpKeys = new Dictionary<string, bool>();

        public List<StateObject> GameStates;

        public int PlayerToken;
        public bool Restart;

        public GameState() { }

        public GameState(List<StateObject> gameStates, int playerToken)
        {
            GameStates = gameStates;

            foreach (var state  in InputControlller.KeyValuePairsPush)
            {
                if (state.Value)
                {
                    this.PushKeys.Add(state.Key, state.Value);
                }
            }

            foreach (var state in InputControlller.KeyValuePairsUp)
            {
                if (state.Value)
                {
                    this.UpKeys.Add(state.Key, state.Value);
                }
            }

            foreach (var state in InputControlller.KeyValuePairsDown)
            {
                if (state.Value)
                {
                    this.DownKeys.Add(state.Key, state.Value);
                }
            }

            PlayerToken = playerToken;
        }
    }
}
