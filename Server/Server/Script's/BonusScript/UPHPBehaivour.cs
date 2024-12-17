using GameObjects;
using OutputWindow.Scripts.TankScripts;

namespace OutputWindow.Scripts.BonusScript
{
    public class UPHPBehaivour : BonusBehaivour
    {
        public UPHPBehaivour(GameObject mesh) : base(mesh)
        {
        }

        public override void SetBonus()
        {
            if (_target is PlayerTankScript player)
            {
                player.SetNewHP(10f);
            }
        }

        public override void RemoveBonus()
        {
            if (_target is PlayerTankScript player)
            {
                player.SetNewHP(-10f);
            }
        }
    }
}
