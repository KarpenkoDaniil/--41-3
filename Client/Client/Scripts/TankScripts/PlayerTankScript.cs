using Behaivourus;
using GameLoader;
using GameObjects;
using IputController;
using OutputWindow.Network;
using Server.GameObject.Interface;

namespace OutputWindow.Scripts.TankScripts
{
    public class PlayerTankScript : MonoBehaivour
    {
        public float CurrentArmor = 0f;
        public float MaxHealth = 100;
        public float CurrentHealth = 100;
        public float CurrentDamage = 10f;
        public float MaxGas = 300;
        public float CurentGas = 150;
        public float Veilocity = 0.5f;
        public float DirX;
        public float DirY;
        public float DirZ;
        public int Target;
        public int PlayerToken;
        public bool GameStatusEnd = false;

        private UserInterface2D interface2D;
        private UserInterface2D winOrDefeat;

        public PlayerTankScript(GameObject mesh) : base(mesh)
        {
            GameObject.SimpleCollision.TurnOfOn();

            interface2D = new UserInterface2D();
            interface2D.X_Position = 10;
            interface2D.Y_Position = 10;

            winOrDefeat = new UserInterface2D();
            winOrDefeat.X_Position = 200;
            winOrDefeat.Y_Position = 200;
        }

        public override void Start(object sender, EventArgs e)
        {

        }

        public override void Colision(object sender, EventArgs e)
        {

        }

        public override void Update(object sender, EventArgs e)
        {
            StateObject stateObject = new StateObject(GameObject, StateObject.TypeOfMesage.Read);

            if (PlayerToken == SingeltonEngine.ConectionManager.PlayerToken)
            {
                interface2D.Text = "";
                interface2D.Text = "HP: " + CurrentHealth + "\n";
                interface2D.Text = interface2D.Text + "Fuel Gas: " + CurentGas + "\n";
                interface2D.Text = interface2D.Text + "Damage: " + CurrentDamage + "\n";
                interface2D.Text = interface2D.Text + "Armor: " + CurrentArmor + "\n";
            }

            if (GameStatusEnd)
            {
                if (CurrentHealth <= 0 || CurentGas <= 0)
                {
                    winOrDefeat.Text = "Your Defeat";
                }
                else
                {
                    winOrDefeat.Text = "Your Win";
                }
            }

            if (InputControlller.PushedButton(OpenTK.Input.Key.A))
            {
                SingeltonEngine.ConectionManager.AddState(stateObject);
            }
            else if (InputControlller.PushedButton(OpenTK.Input.Key.D))
            {
                SingeltonEngine.ConectionManager.AddState(stateObject);
            }
            else if (InputControlller.PushedButton(OpenTK.Input.Key.S))
            {
                SingeltonEngine.ConectionManager.AddState(stateObject);
            }
            else if (InputControlller.PushedButton(OpenTK.Input.Key.W))
            {
                SingeltonEngine.ConectionManager.AddState(stateObject);
            }

            if (InputControlller.PushedButton(OpenTK.Input.Key.Space))
            {
                SingeltonEngine.ConectionManager.AddState(stateObject);
            }
        }

        public override void Delete()
        {
            SingeltonEngine.Render.RemoveFromRender2DUI(interface2D);
        }

        public void SetNewHP(float deltaHP)
        {
            MaxHealth += deltaHP;
        }

        public void SetNewGas(float deltaGas)
        {
            CurentGas += deltaGas;
        }

        public void SetDeltaArmmor(float deltaArmmor)
        {
            CurrentArmor += deltaArmmor;
        }

        public void SetDeltaDamage(float deltaDameg)
        {
            CurrentDamage += deltaDameg;
        }
    }
}
