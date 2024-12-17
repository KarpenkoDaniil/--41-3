using Game;
using NetWorkConnector;
using Newtonsoft.Json;
using OutputWindow.Game;
using System.Net;

namespace Server
{
    public class Programm
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Server Start");
            string pathToConfServer = AppDomain.CurrentDomain.BaseDirectory + "ConfServer.json";  
            string pathToMap = AppDomain.CurrentDomain.BaseDirectory + "CurentMap.json";
            string cfgFileMap = File.ReadAllText(pathToMap);
            string cfgFileServer = File.ReadAllText(pathToConfServer);

            var dictonaryMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(cfgFileMap);
            var dictonaryServer = JsonConvert.DeserializeObject<Dictionary<string, string>>(cfgFileServer);

            IPAddress ipAddress = IPAddress.Parse(dictonaryServer["IPADRES"]);
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, int.Parse(dictonaryServer["PORT"]));

            string map = AppDomain.CurrentDomain.BaseDirectory + "MetaData\\Map's\\" + dictonaryMap["Map"];

            ConectionManager conectionManager = new ConectionManager();
            SingeltonEngine.SetConectionMenager(conectionManager);
            conectionManager.SetHost(true, iPEndPoint);

            LoaderTexture loaderTexture = new LoaderTexture();
            loaderTexture.LoadTextures();

            ObjectLoader objectLoader = new ObjectLoader(map, loaderTexture.Textures);
            GameEngine gameEngine = new GameEngine(objectLoader);
        }
    }
}
