using GameObjects;
using OpenTK;
using OutputWindow.Network;
using OutputWindow;
using OutputWindow.Scripts.TankScripts;
using Server.GameObject.Interface;

namespace GameLoader
{
    public class ObjectLoader
    {
        string _pathToMap;
        private List<Texture> _textures;
        private List<string> _nameOfObjects = new List<string>();
        public List<string> NameOfObjects => _nameOfObjects;
        public ObjectLoader(List<Texture> textures)
        {
            _textures = textures;

            foreach (Texture tex in textures)
            {
                _nameOfObjects.Add(tex.Name);
            }
        }

        public Texture GetTexturByID(int ID)
        {
            foreach (var item in _textures)
            {
                if (item.Index == ID)
                {
                    return item;
                }
            }
            return null;
        }

        public GameObject CreateObject(string nameOfObject)
        {
            Vector3 pos = Vector3.Zero;
            Mesh mesh = GetMesh(pos);
            Texture texture = null;
            foreach (var listText in _textures)
            {
                if (listText.Name == nameOfObject)
                {
                    texture = listText;
                    break;
                }
            }
            GameObject gameObject = new GameObject(mesh, texture);
            return gameObject;
        }

        public GameObject CreateObject(string nameOfObject, List<StateScript> stateScripts)
        {
            Vector3 pos = Vector3.Zero;
            Mesh mesh = GetMesh(pos);
            Texture texture = null;
            foreach (var listText in _textures)
            {
                if (listText.Name == nameOfObject)
                {
                    texture = listText;
                    break;
                }
            }
            GameObject gameObject = new GameObject(mesh, texture, stateScripts);
            return gameObject;
        }

        public GameObject CreateObject(string nameOfObject, StateObject.TypeOfMesage netType)
        {
            Vector3 pos = Vector3.Zero;
            Mesh mesh = GetMesh(pos);
            Texture texture = null;
            foreach (var listText in _textures)
            {
                if (listText.Name == nameOfObject)
                {
                    texture = listText;
                    break;
                }
            }
            GameObject gameObject = new GameObject(mesh, texture);
            return gameObject;
        }

        private Vector3 GetCenterOfObject(float stepX, float stepY, float x, float y)
        {
            Vector3 objectPos = Vector3.Zero;
            objectPos.X = -1 + (x + 0.5f) * stepX;
            objectPos.Y = -1 + (y + 0.5f) * stepY;

            return objectPos;
        }

        private Mesh GetMesh(Vector3 pos)
        {
            Vector3[] vercites = new Vector3[4];

            //First Face
            vercites[0] = new Vector3(1f, 1f, 0f);
            vercites[1] = new Vector3(1f, -1f, 0f);
            vercites[2] = new Vector3(-1f, -1f, 0f);
            vercites[3] = new Vector3(-1f, 1f, 0f);

            return new Mesh(pos, vercites);
        }

        // метод для вычисления координат для объекта
        private Vector2[] GetPosition(Vector2 firstPoint, float coefX, float coefY, float x, float y)
        {
            firstPoint = new Vector2(x, y);
            Vector2[] objectVercites = new Vector2[]
            {
                new Vector2(firstPoint.X - coefX, firstPoint.Y),
                new Vector2(firstPoint.X,firstPoint.Y),
                new Vector2(firstPoint.X - coefY,firstPoint.Y + coefY),
                new Vector2(firstPoint.X ,firstPoint.Y + coefY)
            };
            return objectVercites;
        }
    }
}
