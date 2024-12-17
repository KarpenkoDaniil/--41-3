using Behaivourus;
using GameObjects;
using OpenTK;
using OutputWindow.Game;
using OutputWindow.Network;
using OutputWindow.Scripts.BonusScript;
using OutputWindow.Scripts.TankScripts;
using Server.Script_s.GameScript;
using Server.Script_s.SwampScript;
using System.Drawing;

namespace Game
{
    public class ObjectLoader
    {
        string _pathToMap;
        private List<Texture> _textures;
        private List<string> _nameOfObjects = new List<string>();
        public List<string> NameOfObjects => _nameOfObjects;
        public ObjectLoader(string file, List<Texture> textures)
        {
            _pathToMap = file;
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

        public List<GameObject> Load()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            using (Bitmap map = new Bitmap(_pathToMap))
            {
                float stepX = 2 / (float)map.Width;
                float stepY = 2 / (float)map.Width;
                Color tank_1 = Color.FromArgb(0, 128, 0);
                Color tank_2 = Color.FromArgb(0, 0, 128);
                Color gas = Color.FromArgb(255, 0, 0);
                Color wall = Color.FromArgb(0, 0, 0);
                Color bonus = Color.FromArgb(255, 255, 0);
                Color armor = Color.FromArgb(192, 192, 192);
                Color ammo = Color.FromArgb(255, 0, 255);
                Color swamp = Color.FromArgb(181, 230, 29);

                Mesh mesh1 = GetMesh(Vector3.Zero);
                mesh1.ScaleTo(1f);
                Texture texture = null;

                foreach (var item in _textures)
                {
                    if (item.Name.Contains("mapTex"))
                    {
                        texture = item;
                        break;
                    }
                }

                GameObject gameObject = new GameObject(mesh1, texture);
                gameObject.Collision.isTurn = false;

                GameScript gameScript = new GameScript(gameObject);
                gameObject.AddBehaivour(gameScript);
                
                gameObjects.Add(gameObject);

                for (int i = 0; i < map.Width; i++)
                {
                    for (int j = 0; j < map.Height; j++)
                    {
                        Vector3 position = GetCenterOfObject(stepX, stepY, i, j);

                        if (map.GetPixel(i, j) == swamp)
                        {
                            foreach (var listText in _textures)
                            {
                                if (listText.Name.Contains("swamp"))
                                {
                                    Mesh mesh = GetMesh(position);
                                    mesh.Layer = 1;
                                    mesh.ScaleTo(1.0f / map.Width);
                                    gameObject = new GameObject(mesh, listText);
                                    gameObjects.Add(gameObject);
                                    gameObject.NameObject = "swamp";
                                    gameObject.isStatic = false;
                                    gameObject.MonoBehaivours.Add(new SwampScript(gameObject));
                                    gameObject.SimpleCollision.TurnOfOn();
                                    gameObject.Collision.isTriger = true;
                                    break;
                                }
                            }
                        }

                        if (map.GetPixel(i, j) == tank_1)
                        {
                            foreach (var listText in _textures)
                            {
                                if (listText.Name.Contains("TankT72_Green"))
                                {
                                    Mesh mesh = GetMesh(position);
                                    mesh.Layer = 2;
                                    mesh.ScaleTo(1.0f / map.Width);
                                    gameObject = new GameObject(mesh, listText);
                                    gameObjects.Add(gameObject);
                                    gameObject.NameObject = "TankT72_Green";
                                    gameObject.Mesh.ScaleTo(gameObject.Mesh.Scale - gameObject.Mesh.Scale * 0.20f);
                                    gameObject.MonoBehaivours.Add(new PlayerTankScript(gameObject));
                                    break;
                                }
                            }
                        }
                        else if (map.GetPixel(i, j) == tank_2)
                        {
                            foreach (var listText in _textures)
                            {
                                if (listText.Name.Contains("TankT72_Red"))
                                {
                                    Mesh mesh = GetMesh(position);
                                    mesh.Layer = 2;
                                    mesh.ScaleTo(1.0f / map.Width);
                                    gameObject = new GameObject(mesh, listText);
                                    gameObjects.Add(gameObject);
                                    gameObject.NameObject = "TankT72_Red";
                                    gameObject.Mesh.ScaleTo(gameObject.Mesh.Scale - gameObject.Mesh.Scale * 0.20f);
                                    gameObject.MonoBehaivours.Add(new PlayerTankScript(gameObject));
                                    break;
                                }
                            }
                        }
                        else if (map.GetPixel(i, j) == wall)
                        {
                            foreach (var listText in _textures)
                            {
                                if (listText.Name.Contains("wall"))
                                {
                                    Mesh mesh = GetMesh(position);
                                    mesh.Layer = 0;
                                    mesh.ScaleTo(1.0f / map.Width);
                                    gameObject = new GameObject(mesh, listText);
                                    gameObjects.Add(gameObject);
                                    gameObject.isStatic = true;
                                    gameObject.NameObject = "Wall";
                                    break;
                                }
                            }
                        }
                        else if (map.GetPixel(i, j) == bonus)
                        {
                            foreach (var listText in _textures)
                            {
                                if (listText.Name.Contains("BonusBox"))
                                {
                                    Mesh mesh = GetMesh(position);
                                    mesh.Layer = 1;
                                    mesh.ScaleTo(1.0f / map.Width);
                                    gameObject = new GameObject(mesh, listText);
                                    gameObjects.Add(gameObject);
                                    gameObject.NameObject = "BonusBox";
                                    gameObject.isStatic = false;
                                    gameObject.MonoBehaivours.Add(new BonusSpawnerScript(gameObject));
                                    break;
                                }
                            }
                        }
                        else if (map.GetPixel(i, j) == gas)
                        {
                            foreach (var listText in _textures)
                            {
                                if (listText.Name.Contains("Fuel_Can"))
                                {
                                    Mesh mesh = GetMesh(position);
                                    mesh.Layer = 1;
                                    mesh.ScaleTo(1.0f / map.Width);
                                    gameObject = new GameObject(mesh, listText);
                                    gameObjects.Add(gameObject);
                                    gameObject.isStatic = false;
                                    gameObject.NameObject = "Fuel_Can";
                                    gameObject.MonoBehaivours.Add(new FullGasBehaivour(gameObject));
                                    break;
                                }
                            }
                        }
                        else if (map.GetPixel(i, j) == ammo)
                        {
                            foreach (var listText in _textures)
                            {
                                if (listText.Name.Contains("AmmoBox"))
                                {
                                    Mesh mesh = GetMesh(position);
                                    mesh.Layer = 1;
                                    mesh.ScaleTo(1.0f / map.Width);
                                    gameObject = new GameObject(mesh, listText);
                                    gameObjects.Add(gameObject);
                                    gameObject.isStatic = false;
                                    gameObject.NameObject = "AmmoBox";
                                    gameObject.MonoBehaivours.Add(new FullGasBehaivour(gameObject));
                                    break;
                                }
                            }
                        }
                        else if (map.GetPixel(i, j) == armor)
                        {
                            foreach (var listText in _textures)
                            {
                                if (listText.Name.Contains("shield"))
                                {
                                    Mesh mesh = GetMesh(position);
                                    mesh.Layer = 1;
                                    mesh.ScaleTo(1.0f / map.Width);
                                    gameObject = new GameObject(mesh, listText);
                                    gameObject.isStatic = true;
                                    gameObjects.Add(gameObject);
                                    gameObject.NameObject = "shield";
                                    gameObject.MonoBehaivours.Add(new ArmorBehaivour(gameObject));
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return gameObjects;
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
            SingeltonEngine.GameEngine.AddObject(gameObject);
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
            if (StateObject.TypeOfMesage.Create == netType)
            {
                SingeltonEngine.GameEngine.AddObject(gameObject);
            }
            return gameObject;
        }

        public GameObject CreateObject(string nameOfObject, List<Type> TypeOfScripts)
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
            object[] objects = { gameObject };

            foreach (var type in TypeOfScripts)
            {
                gameObject.AddBehaivour((Behaivourus.MonoBehaivour)Activator.CreateInstance(type, gameObject));
            }

            SingeltonEngine.GameEngine.AddObject(gameObject);
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
