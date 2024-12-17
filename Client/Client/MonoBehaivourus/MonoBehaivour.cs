using GameLoader;
using GameObjects;
using Newtonsoft.Json;
using OpenTK;
using System.Reflection;

namespace Behaivourus
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IgnorePropertyAttribute : Attribute { }

    public class MonoBehaivour
    {
        [JsonIgnore]
        [IgnoreProperty]
        public GameObject GameObject
        {
            get { return gameObject; }
            set { gameObject = value; }
        }

        [IgnoreProperty]
        [JsonIgnore]
        protected GameObject gameObject;
        [IgnoreProperty]
        [JsonIgnore]
        protected Vector3 possion => gameObject.Position;
        [IgnoreProperty]

        protected List<object> _attributes = new List<object>();

        public MonoBehaivour(GameObject mesh)
        {
            gameObject = mesh;
            gameObject.Collision.AddListener(this);
        }

        public virtual void Start(object sender, EventArgs e)
        {

        }

        public virtual void Update(object sender, EventArgs e)
        {

        }

        public virtual void Colision(object sender, EventArgs e)
        {

        }

        public virtual void Trigger(object sender, EventArgs e)
        {

        }

        protected void SereilizeScript(Type type)
        {
            _attributes.Clear();
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldValue = field.GetValue(this); // Получаем значение поля
                _attributes.Add(fieldValue);  // Добавляем в коллекцию
            }
        }

        public List<object> ReturnSatate(Type type)
        {
            SereilizeScript(type);
            return _attributes;
        }

        public void SateState(List<object> list)
        {
            Type type = this.GetType();
            int index = 0;
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldType = field.FieldType;
                var value = list[index];

                if (value is Vector3)
                {
                    field.SetValue(this, list[index]); // Получаем значение поля
                    index++;
                }
                else
                {
                    var valueConv = Convert.ChangeType(list[index], fieldType);
                    field.SetValue(this, valueConv); // Получаем значение поля
                    index++;
                }
            }
        }

        protected void RemoveScript()
        {
            GameObject.MonoBehaivours.Remove(this);
            SingeltonEngine.GameClient.RemoveListeners(this);
        }

        public virtual void Delete()
        {
        }

        public virtual MonoBehaivour Clone()
        {
            return new MonoBehaivour(GameObject);
        }
    }
}
