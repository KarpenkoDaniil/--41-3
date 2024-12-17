using Behaivourus;
using GameObjects;
using OutputWindow.Game;
using OutputWindow.Scripts.TankScripts;

namespace Server.Script_s.GameScript
{
    public class GameScript : MonoBehaivour
    {
        private List<PlayerTankScript> _players = new List<PlayerTankScript>();
        private bool _gameIsStoped = false;
        public GameScript(GameObject mesh) : base(mesh)
        {
            _copyScript = false;
        }

        public override void Start(object sender, EventArgs e)
        {
            var list = SingeltonEngine.GameEngine.GameObjects;
            foreach (var gameObject in list)
            {
                foreach (var item in gameObject.MonoBehaivours)
                {
                    if (item is PlayerTankScript script)
                    {
                        _players.Add(script);
                        break;
                    }
                }
            }
        }

        float _timeToRestart = 3;
        float _timer = 0;

        public override void Update(object sender, EventArgs e)
        {
            if (!_gameIsStoped)
            {
                foreach (var item in _players)
                {
                    if (item.CurentGas <= 0 || item.CurrentHealth <= 0)
                    {
                        StopGame();
                        break;
                    }
                }
            }
            else
            {
                if (_timer >= _timeToRestart)
                {
                    _timer = 0;
                    _gameIsStoped = false;
                    SingeltonEngine.GameEngine.Restart();
                }
                else
                {
                    _timer = _timer + TimeDeltaHelper.DeltaT;
                }
            }
        }

        private void StopGame()
        {
            _gameIsStoped = true;
            foreach (var player in _players)
            {
                player.TankDestroy();
            }
            _players.Clear();
        }
    }
}
