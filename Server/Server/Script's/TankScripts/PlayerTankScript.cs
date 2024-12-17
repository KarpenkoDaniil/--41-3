using Behaivourus;
using GameObjects;
using Newtonsoft.Json.Linq;
using OpenTK;
using OutputWindow.Game;
using Server.GameLogic.InputController;
using Server.MonoBehaivourus;
using Vector3 = OpenTK.Vector3;

namespace OutputWindow.Scripts.TankScripts
{
    public class PlayerTankScript : MonoBehaivour, OnlinePlaybleBehaivour
    {
        public float CurrentArmor = 0f;
        public float MaxHealth = 100;
        public float CurrentHealth = 100;
        public float CurrentDamage = 10f;
        public float MaxGas = 5000;
        public float CurentGas = 1500;
        public float Veilocity = 0.5f;
        public float DirX = -1;
        public float DirY = 0;
        public float DirZ = 0;
        public int Target;
        public int PlayerToken;
        public bool GameStatusEnd = false;

        private Vector3 DirectionOfMove => new Vector3(DirX, DirY, DirZ);
        private float _timerToFire = 0;
        private bool _canFire = true;
        private bool _canMove = true;
        private int _playerToken = 0;
        private bool inSwamp = false;
        public PlayerTankScript(GameObject mesh) : base(mesh)
        {
            GameObject.SimpleCollision.TurnOfOn();
        }

        public void SetToken(int token)
        {
            _playerToken = token;
        }

        public int GetToken()
        {
            return _playerToken;
        }

        public void TankDestroy()
        {
            _canFire = false;
            _canMove = false;
            GameStatusEnd = true;
            SingeltonEngine.ConectionManager.AddObjectToSend(this.GameObject, Network.StateObject.TypeOfMesage.Read);
        }

        public override void Start(object sender, EventArgs e)
        {
            if (GameObject.NameObject.Contains("TankT72_Green"))
            {
                GameObject gameObject = SingeltonEngine.GameEngine.GetGameObjectByName("TankT72_Red");

                if (gameObject != null)
                {
                    Target = gameObject.GameObjectID;
                }
            }
            else
            {
                GameObject gameObject = SingeltonEngine.GameEngine.GetGameObjectByName("TankT72_Green");

                if (gameObject != null)
                {
                    Target = gameObject.GameObjectID;
                }
            }
        }

        public override void Colision(object sender, EventArgs e)
        {
            if (sender is Collision collision)
            {
                foreach (var monoBehaivour in collision.gameObject.MonoBehaivours)
                {
                    if (monoBehaivour is ShellScript.ShellBehaivour shellBehaivour)
                    {
                        if (shellBehaivour.Target != Target)
                        {
                            CurrentHealth = CurrentHealth - shellBehaivour.SizeOfDamage;
                            SingeltonEngine.ConectionManager.AddObjectToSend(GameObject, Network.StateObject.TypeOfMesage.Read);
                        }
                    }
                }
            }
        }

        public bool PlayerAtSwamp()
        {
            return inSwamp;
        }

        public void AtSwamp()
        {
            inSwamp = true;
        }

        public void OutSwamp()
        {
            inSwamp = false;
        }

        private float _timeToSand = 3f; 
        private float _timer = 0;

        public override void Update(object sender, EventArgs e)
        {
            if (CurentGas < 0)
            {
                CurentGas = 0;
            }

            float curVeilocity = Veilocity;
            if (inSwamp)
            {
                curVeilocity = curVeilocity - curVeilocity * 0.5f;
            }

            if (PlayerInputController.PlayerToken == _playerToken && _canMove)
            {
                PlayerToken = _playerToken;
                if (PlayerInputController.PushedButton(OpenTK.Input.Key.A))
                {
                    DirX = -1;
                    DirY = 0;
                    DirZ = 0;
                    GameObject.Mesh.Roll = -MathHelper.Pi;
                    GameObject.SimpleCollision.AddImpact(curVeilocity, DirectionOfMove);
                    CurentGas = CurentGas - 0.1f;
                    SingeltonEngine.ConectionManager.AddObjectToSend(GameObject, Network.StateObject.TypeOfMesage.Read);
                }
                else if (PlayerInputController.PushedButton(OpenTK.Input.Key.D))
                {
                    DirX = 1;
                    DirY = 0;
                    DirZ = 0;
                    GameObject.Mesh.Roll = 0;
                    GameObject.SimpleCollision.AddImpact(curVeilocity, DirectionOfMove);
                    CurentGas = CurentGas - 0.1f;
                    SingeltonEngine.ConectionManager.AddObjectToSend(GameObject, Network.StateObject.TypeOfMesage.Read);
                }
                else if (PlayerInputController.PushedButton(OpenTK.Input.Key.S))
                {
                    DirX = 0;
                    DirY = -1;
                    DirZ = 0;
                    GameObject.Mesh.Roll = 3 * MathHelper.Pi / 2;
                    GameObject.SimpleCollision.AddImpact(curVeilocity, DirectionOfMove);
                    CurentGas = CurentGas - 0.1f;
                    SingeltonEngine.ConectionManager.AddObjectToSend(GameObject, Network.StateObject.TypeOfMesage.Read);
                }
                else if (PlayerInputController.PushedButton(OpenTK.Input.Key.W))
                {
                    DirX = 0;
                    DirY = 1;
                    DirZ = 0;
                    GameObject.Mesh.Roll = MathHelper.Pi / 2;
                    GameObject.SimpleCollision.AddImpact(curVeilocity, DirectionOfMove);
                    CurentGas = CurentGas - 0.1f;
                    SingeltonEngine.ConectionManager.AddObjectToSend(GameObject, Network.StateObject.TypeOfMesage.Read);
                }

                if (PlayerInputController.PushedButton(OpenTK.Input.Key.Space) && _canFire)
                {
                    Fire();
                }
            }

            DelayOfFire();
        }

        private float _countOfDelay = 0;
        private float _delayOfFire = 0.25f;

        private void DelayOfFire()
        {
            if (!_canFire)
            {
                _countOfDelay = _countOfDelay + TimeDeltaHelper.DeltaT;
                if (_countOfDelay > _delayOfFire)
                {
                    _canFire = true;
                    _countOfDelay = 0;
                }
            }
        }

        private void Fire()
        {
            _canFire = false;
            var shell = CreateShell();
            shell.Mesh.Layer = 2;
            shell.Mesh.Roll = GameObject.Mesh.Roll;
            shell.AddBehaivour(new ShellScript.ShellBehaivour(shell));
            shell.Mesh.ScaleTo(GameObject.Mesh.Scale / 3);
            shell.Mesh.SetPosition(GameObject.Position);

            foreach (var monoBehaivour in shell.MonoBehaivours)
            {
                if (monoBehaivour is ShellScript.ShellBehaivour shellBehaivour)
                {
                    shellBehaivour.InitScript(DirectionOfMove, Target, CurrentDamage, Veilocity * 1.5f);
                }
            }
            SingeltonEngine.ConectionManager.AddObjectToSend(shell, Network.StateObject.TypeOfMesage.Create);
        }

        public GameObject CreateShell()
        {
            string nameObject = "AP";

            foreach (var item in SingeltonEngine.ObjectLoader.NameOfObjects)
            {
                if (item.Contains(nameObject))
                {
                    nameObject = item;
                    break;
                }
            }

            return SingeltonEngine.ObjectLoader.CreateObject(nameObject);
        }

        public void SetNewHP(float deltaHP)
        {
            MaxHealth += deltaHP;
            SingeltonEngine.ConectionManager.AddObjectToSend(this.GameObject, Network.StateObject.TypeOfMesage.Read);
        }

        public void SetNewGas(float deltaGas)
        {
            CurentGas += deltaGas;
            SingeltonEngine.ConectionManager.AddObjectToSend(this.GameObject, Network.StateObject.TypeOfMesage.Read);
        }

        public void SetDeltaArmmor(float deltaArmmor)
        {
            CurrentArmor += deltaArmmor;
            SingeltonEngine.ConectionManager.AddObjectToSend(this.GameObject, Network.StateObject.TypeOfMesage.Read);
        }

        public void SetDeltaDamage(float deltaDameg)
        {
            CurrentDamage += deltaDameg;
            SingeltonEngine.ConectionManager.AddObjectToSend(this.GameObject, Network.StateObject.TypeOfMesage.Read);
        }

        public override MonoBehaivour Clone()
        {
            return base.Clone();
        }
    }
}
