using Behaivourus;
using Newtonsoft.Json;
using OpenTK;
using OutputWindow.GameObjects;
using GameLoader;
using OutputWindow.Network;

namespace GameObjects
{
    public class GameObject
    {
        [JsonIgnore] private Texture _texture;
        public Texture Texture => _texture;

        [JsonProperty] private Mesh _mesh;
        public Mesh Mesh => _mesh;

        [JsonProperty] private List<MonoBehaivour> _monoBehaivours = new List<MonoBehaivour>();
        public List<MonoBehaivour> MonoBehaivours => _monoBehaivours;

        [JsonIgnore] private Collision _collision;
        public Collision Collision => _collision;

        [JsonIgnore] private SimpleCollision _simpleCollision;
        public SimpleCollision SimpleCollision => _simpleCollision;

        public Vector3 Position => _mesh.Position;

        public Matrix4 Matrix => _mesh.GetWorldMatrix();

        [JsonProperty] public string NameObject = "Object";
        [JsonProperty] public int GameObjectID;

        public bool IsVisible = true;   

        public GameObject(Mesh mesh, Texture texture)
        {
            _texture = texture;
            _mesh = mesh;
            _collision = new Collision(this);
            AddBehaivour(new MonoBehaivour(this));
            GameObjectID = this.GetHashCode();
            _simpleCollision = new SimpleCollision(this);
            _simpleCollision.TurnOfOn();
        }

        public GameObject(Mesh mesh, Texture texture, MonoBehaivour monoBehaivour)
        {
            _texture = texture;
            _mesh = mesh;
            _collision = new Collision(this);
            _simpleCollision = new SimpleCollision(this);
            _simpleCollision.TurnOfOn();
            AddBehaivour(monoBehaivour);
        }

        public GameObject(Mesh mesh, Texture texture , List<StateScript> stateScripts)
        {
            _texture = texture;
            _mesh = mesh;
            _collision = new Collision(this);
            _simpleCollision = new SimpleCollision(this);
            _simpleCollision.TurnOfOn();

            foreach (var script in stateScripts)
            {
                Type type = Type.GetType(script.TypeOfScript);
                var behaivour = (MonoBehaivour)Activator.CreateInstance(type, this);
                AddBehaivour(behaivour);
            }
        }

        public void SetTexture(Texture texture)
        {
            _texture = texture;
        }

        public void AddBehaivour(MonoBehaivour monoBehaivour)
        {
            _monoBehaivours.Add(monoBehaivour);
            _collision.AddListener(monoBehaivour);
            //SingeltonEngine.GameClient.AddListenerToUpdate(monoBehaivour);
        }
    }
}
