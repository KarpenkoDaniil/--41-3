using Behaivourus;
using Newtonsoft.Json;
using OpenTK;
using OutputWindow.Game;
using OutputWindow.GameObjects;

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

        public bool IsDestroyed => _isDestroyed;
        private bool _isDestroyed = false;

        public bool IsVisible = true;
        public bool isStatic = false;

        public GameObject(Mesh mesh, Texture texture)
        {
            _texture = texture;
            _mesh = mesh;
            _collision = new Collision(this);
            _monoBehaivours.Add(new MonoBehaivour(this));
            GameObjectID = this.GetHashCode();
            _simpleCollision = new SimpleCollision(this);
            _simpleCollision.TurnOfOn();
        }

        public GameObject(Mesh mesh, Texture texture, MonoBehaivour monoBehaivour)
        {
            _texture = texture;
            _mesh = mesh;
            _collision = new Collision(this);
            _monoBehaivours.Add(monoBehaivour);
            _simpleCollision = new SimpleCollision(this);
            _simpleCollision.TurnOfOn();
        }

        public void SetTexture(Texture texture)
        {
            _texture = texture;
        }

        public void AddBehaivour(MonoBehaivour monoBehaivour)
        {
            _monoBehaivours.Add(monoBehaivour);
            _collision.AddListener(monoBehaivour);
            SingeltonEngine.GameEngine.AddListener(monoBehaivour);
        }

        public void DestroyObject()
        {
            _isDestroyed = true;
        }
    }
}
