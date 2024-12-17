using Behaivourus;
using GameObjects;
using OpenTK;
using OutputWindow.Game;

namespace OutputWindow.Scripts.ShellScript
{
    public class ShellBehaivour : MonoBehaivour
    {
        public int Target;
        private Vector3 Direction;
        public float SizeOfDamage;
        public float VeilocityOfShell;

        public ShellBehaivour(GameObject mesh) : base(mesh)
        {
            GameObject.Collision.isTriger = true;
            GameObject.SimpleCollision.TurnOfOn();
            _copyScript = false;
        }

        public void InitScript(Vector3 direction, int target, float damage, float veilocity)
        {
            Target = target;
            Direction = direction;
            SizeOfDamage = damage;
            VeilocityOfShell = veilocity;
        }

        public override void Colision(object sender, EventArgs e)
        {
            if (sender is Collision collision)
            {
                if (collision.gameObject.NameObject == "Wall")
                {
                    this.Delete();
                    SingeltonEngine.ConectionManager.AddObjectToSend(this.GameObject, Network.StateObject.TypeOfMesage.Delete);
                }
                else if (Target == collision.gameObject.GameObjectID)
                {
                    this.Delete();
                    SingeltonEngine.ConectionManager.AddObjectToSend(this.GameObject, Network.StateObject.TypeOfMesage.Delete);
                }
            }
        }

        int _every = 2;
        int _count = 0;

        public override void Update(object sender, EventArgs e)
        {
            GameObject.SimpleCollision.AddImpact(VeilocityOfShell, Direction);
            if (_count == 3)
            {
                SingeltonEngine.ConectionManager.AddObjectToSend(GameObject, Network.StateObject.TypeOfMesage.Read);
                _count = 0;
            }
            _count++;
        }

        public override MonoBehaivour Clone()
        {
            return base.Clone();
        }
    }
}
