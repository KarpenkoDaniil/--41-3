using OpenTK.Input;

namespace Server.GameLogic.InputController
{
    public class PlayerInputController
    {
        public  Dictionary<string, bool> KeyValuePairsPush => _keyValuePairsPush;
        private  Dictionary<string, bool> _keyValuePairsPush = new Dictionary<string, bool>();

        public  Dictionary<string, bool> KeyValuePairsUp => _keyValuePairsUp;
        private  Dictionary<string, bool> _keyValuePairsUp = new Dictionary<string, bool>();

        public  Dictionary<string, bool> KeyValuePairsDown => _keyValuePairsDown;
        private  Dictionary<string, bool> _keyValuePairsDown = new Dictionary<string, bool>();

        private int _playerToken;
        public int PlayerToken => _playerToken;

        public PlayerInputController()
        {
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                _keyValuePairsPush[key.ToString()] = false;
                _keyValuePairsUp[key.ToString()] = false;
                _keyValuePairsDown[key.ToString()] = false;
            }
        }

        public PlayerInputController(int token)
        {
            _playerToken = token;
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                _keyValuePairsPush[key.ToString()] = false;
                _keyValuePairsUp[key.ToString()] = false;
                _keyValuePairsDown[key.ToString()] = false;
            }
        }

        public bool PushedButton(Key key)
        {
            var t = key.ToString();
            return _keyValuePairsPush[key.ToString()];
        }

        public bool UpButton(Key key)
        {
            var v = _keyValuePairsUp[key.ToString()];
            return _keyValuePairsUp[key.ToString()];
        }

        public bool DownButton(Key key)
        {
            return _keyValuePairsDown[key.ToString()];
        }

        public void UpdateInput()
        {
            // Сбрасываем состояния Up и Down на каждом кадре
            foreach (var key in _keyValuePairsUp.Keys.ToList())
            {
                _keyValuePairsUp[key] = false;
            }

            foreach (var key in _keyValuePairsDown.Keys.ToList())
            {
                _keyValuePairsDown[key] = false;
            }

            foreach (var key in _keyValuePairsPush.Keys.ToList())
            {
                _keyValuePairsPush[key] = false;
            }
        }
    }
}
