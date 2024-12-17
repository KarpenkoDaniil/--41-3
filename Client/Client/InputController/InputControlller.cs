using OpenTK;
using OpenTK.Input;

namespace IputController
{
    public static class InputControlller
    {
        public static Dictionary<string, bool> KeyValuePairsPush => _keyValuePairsPush;
        private static Dictionary<string, bool> _keyValuePairsPush = new Dictionary<string, bool>();

        public static Dictionary<string, bool> KeyValuePairsUp => _keyValuePairsUp;
        private static Dictionary<string, bool> _keyValuePairsUp = new Dictionary<string, bool>();

        public static Dictionary<string, bool> KeyValuePairsDown => _keyValuePairsDown;
        private static Dictionary<string, bool> _keyValuePairsDown = new Dictionary<string, bool>();

        static GLControl _gLControl;

        public static void InitInputConroller(GLControl gLControl)
        {
            _gLControl = gLControl;
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                _keyValuePairsPush[key.ToString()] = false;
                _keyValuePairsUp[key.ToString()] = false;
                _keyValuePairsDown[key.ToString()] = false;
            }

            _gLControl.KeyDown += _gLControl_KeyDown;
            _gLControl.KeyUp += _gLControl_KeyUp;
            _gLControl.KeyPress += _gLControl_KeyPress;
        }

        private static void _gLControl_KeyPress(object? sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            string keyPress = e.KeyChar.ToString().ToUpper();
            _keyValuePairsPush[keyPress] = true;
        }

        private static void _gLControl_KeyUp(object? sender, KeyEventArgs e)
        {
            string keyUp = e.KeyCode.ToString();
            _keyValuePairsPush[keyUp] = false;
            _keyValuePairsUp[keyUp] = true;
        }

        private static void _gLControl_KeyDown(object? sender, KeyEventArgs e)
        {
            string keyDown = e.KeyData.ToString();
            _keyValuePairsPush[keyDown] = true;
            _keyValuePairsDown[keyDown] = true;
        }

        public static bool PushedButton(Key key)
        {
            var t = key.ToString();
            return _keyValuePairsPush[key.ToString()];
        }

        public static bool UpButton(Key key)
        {
            var v = _keyValuePairsUp[key.ToString()];
            return _keyValuePairsUp[key.ToString()];
        }

        public static bool DownButton(Key key)
        {
            return _keyValuePairsDown[key.ToString()];
        }

        public static void UpdateInput()
        {
            // Сбрасываем состояния Up и Down на каждом кадре
            foreach (var key in _keyValuePairsUp.Keys.ToList())
            {
                _keyValuePairsUp[key] = false;
                _keyValuePairsDown[key] = false;
            }
        }
    }
}
