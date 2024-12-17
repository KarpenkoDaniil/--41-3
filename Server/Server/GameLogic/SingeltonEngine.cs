using Game;
using NetWorkConnector;

namespace OutputWindow.Game
{
    public static class SingeltonEngine
    {
        public static GameEngine GameEngine => _gameEngine;
        static GameEngine _gameEngine;

        public static ObjectLoader ObjectLoader => _objectLoader;
        private static ObjectLoader _objectLoader;

        private static ConectionManager _conectionManager;
        public static ConectionManager ConectionManager => _conectionManager;

        static bool _isSetEngine = false;
        static bool _isSetLoader = false;

        public static void SetEngine(GameEngine gameEngine)
        {
            if (!_isSetEngine)
            {
                _isSetEngine = true;
                _gameEngine = gameEngine;
            }
        }
        public static void SetObjectLoader(ObjectLoader loader)
        {
            if (!_isSetLoader)
            {
                _isSetLoader = true;
                _objectLoader = loader;
            }
        }

        public static void SetConectionMenager(ConectionManager conectionManager)
        {
            _conectionManager = conectionManager;
        }
    }
}
