using GameObjects;
using OutputWindow.Game;
using OutputWindow.Scripts.TankScripts;

namespace OutputWindow.Scripts.BonusScript
{
    internal class BonusSpawnerScript : BonusBehaivour
    {
        private float _maxTime = 10;
        private float _curentTime = 0;
        private bool _active = false;

        public BonusSpawnerScript(GameObject mesh) : base(mesh)
        {
            GameObject.Collision.isTriger = true;
            _copyScript = false;
        }

        public override void Colision(object sender, EventArgs e)
        {
            if (sender is Collision collision)
            {
                if (collision.gameObject.NameObject.Contains("Tank"))
                {
                    if (!_active)
                    {
                        Timer();
                        foreach (var script in collision.gameObject.MonoBehaivours)
                        {
                            if (script is PlayerTankScript player)
                            {
                                AbsBonus(player);
                                GameObject.IsVisible = false;
                                SingeltonEngine.ConectionManager.AddObjectToSend(GameObject, Network.StateObject.TypeOfMesage.Read);
                                SingeltonEngine.ConectionManager.AddObjectToSend(player.GameObject, Network.StateObject.TypeOfMesage.Read);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public override void Update(object sender, EventArgs e)
        {
            if (_active)
            {
                if (_curentTime > _maxTime)
                {
                    _active = false;
                    GameObject.IsVisible = true;
                    SingeltonEngine.ConectionManager.AddObjectToSend(GameObject, Network.StateObject.TypeOfMesage.Read);
                }
                _curentTime += TimeDeltaHelper.DeltaT;
            }
        }

        private void Timer()
        {
            _curentTime = 0;
            _active = true;
        }

        private BonusBehaivour AbsBonus(PlayerTankScript gameObject)
        {
            BonusBehaivour bonus = null;
            Random random = new Random();

            int index = random.Next(0, 6);
            switch (index)
            {
                case 0:
                    bonus = new ArmorBehaivour(gameObject.GameObject);
                    break;
                case 1:
                    bonus = new DownDamageBehaivour(gameObject.GameObject);
                    break;
                case 2:
                    bonus = new UPHPBehaivour(gameObject.GameObject);
                    break;
                case 3:
                    bonus = new DownHPBehaivour(gameObject.GameObject);
                    break;
                case 4:
                    bonus = new UpDamageBehaivour(gameObject.GameObject);
                    break;
                case 5:
                    bonus = new FullGasBehaivour(gameObject.GameObject);
                    break;
            }

            Console.WriteLine(bonus.GetType());
            gameObject.GameObject.AddBehaivour(bonus);
            bonus.SetTarget(gameObject);

            return bonus;
        }
    }
}
