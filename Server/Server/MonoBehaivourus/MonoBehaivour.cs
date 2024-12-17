using GameObjects;
using Newtonsoft.Json;
using OpenTK;
using OutputWindow.Game;
using OutputWindow.Network;
using Server.GameLogic.InputController;
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
        protected GameObject gameObject;
        [IgnoreProperty]
        protected Vector3 possion => gameObject.Position;
        [IgnoreProperty]

        protected List<object> _attributes = new List<object>();
        protected PlayerInputController PlayerInputController;
        protected bool _copyScript = true;

        public MonoBehaivour(GameObject mesh)
        {
            gameObject = mesh;
            gameObject.Collision.AddListener(this);
            PlayerInputController = new PlayerInputController();
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

        public bool IsCopingScript()
        {
            return _copyScript;
        }

        public void SateNewInputState(PlayerInputController playerInputController)
        {
            PlayerInputController = playerInputController;
        }

        protected void SereilizeScript(Type type)
        {
            _attributes.Clear();
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!(field.GetValue(this) is MonoBehaivour))
                {
                    var fieldValue = field.GetValue(this); // Получаем значение поля
                    _attributes.Add(fieldValue);  // Добавляем в коллекцию
                }
            }
        }

        public List<object> ReturnSatate(Type type)
        {
            SereilizeScript(type);
            return _attributes;
        }

        public void SateState(List<object> list, PlayerInputController playerInputController, bool modife = true)
        {
            Type type = this.GetType();
            int index = 0;
            PlayerInputController = playerInputController;

            if (modife)
            {
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
        }

        protected void RemoveScript()
        {
            GameObject.MonoBehaivours.Remove(this);
            SingeltonEngine.GameEngine.RemoveListener(this);
        }

        protected void Delete()
        {
            SingeltonEngine.GameEngine.AddObjectToDelete(GameObject);
        }

        public virtual MonoBehaivour Clone()
        {
            return new MonoBehaivour(GameObject);
        }
    }
}
