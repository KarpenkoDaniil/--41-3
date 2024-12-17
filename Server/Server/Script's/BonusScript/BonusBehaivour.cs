using Behaivourus;
using GameObjects;
using OutputWindow.Game;
using OutputWindow.Scripts.TankScripts;

namespace OutputWindow.Scripts.BonusScript
{
    public class BonusBehaivour : MonoBehaivour
    {
        protected MonoBehaivour _target;
        protected float _currentTime = 0;
        protected float _maxTime = 30f;
        protected bool _active = false;
        protected bool _hasTimer = true;

        public BonusBehaivour(GameObject mesh) : base(mesh)
        {
            _copyScript = false;
        }

        public void SetTarget(PlayerTankScript playerTank)
        {
            _target = playerTank;
            _active = true;
            SetBonus();
        }

        public virtual void SetBonus()
        {
        }

        public virtual void RemoveBonus()
        {
        }

        public override void Update(object sender, EventArgs e)
        {
            if (_active)
            {
                if (_currentTime < _maxTime && _hasTimer)
                {
                    _currentTime += TimeDeltaHelper.DeltaT;
                }
                else
                {
                    RemoveBonus();
                    RemoveScript();
                }
            }
        }
    }
}
