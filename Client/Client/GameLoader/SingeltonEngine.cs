using Client.Game;
using NetWorkConnector;
using RenderObject;

namespace GameLoader
{
    public static class SingeltonEngine
    {
        public static ObjectLoader ObjectLoader => _objectLoader;
        private static ObjectLoader _objectLoader;

        private static ConectionManager _conectionManager;
        public static ConectionManager ConectionManager => _conectionManager;

        public static RenderObjects Render => _render;
        private static RenderObjects _render;

        private static LoaderTexture _loaderTexture;
        public static LoaderTexture LoaderTexture => _loaderTexture;

        private static GameClient _gameClient;
        public static GameClient GameClient => _gameClient;

        static bool _isSetEngine = false;
        static bool _isSetLoader = false;
        static bool _isSetRendering = false;
        static bool _isSetLoadTextures = false;
        static bool _isSetGameClient = false;

        public static void SetGameClient(GameClient client)
        {
            if (!_isSetGameClient)
            {
                _gameClient = client;
                _isSetGameClient = true;
            }
        }

        public static void SetLoaderTextures(LoaderTexture loaderTexture)
        {
            if (!_isSetLoadTextures)
            {
                _loaderTexture = loaderTexture;
                _isSetLoadTextures = true;
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
        public static void SetRender(RenderObjects render)
        {
            if (!_isSetRendering)
            {
                _isSetRendering = true;
                _render = render;
            }
        }
        public static void SetConectionMenager(ConectionManager conectionManager)
        {
            _conectionManager = conectionManager;
        }
    }
}
