using GameObjects;
using OutputWindow.Scripts.TankScripts;

namespace OutputWindow.Scripts.BonusScript
{
    public class DownHPBehaivour : BonusBehaivour
    {
        public DownHPBehaivour(GameObject mesh) : base(mesh)
        {
        }

        public override void SetBonus()
        {
            _hasTimer = true;
            if (_target is PlayerTankScript player)
            {
                player.SetNewHP(-10f);
            }
        }

        public override void RemoveBonus()
        {
            if (_target is PlayerTankScript player)
            {
                player.SetNewHP(10f);
            }
        }
    }
}
