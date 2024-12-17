using GameObjects;
using OutputWindow.Scripts.TankScripts;

namespace OutputWindow.Scripts.BonusScript
{
    public class FullGasBehaivour : BonusBehaivour
    {
        public FullGasBehaivour(GameObject mesh) : base(mesh)
        {
            _hasTimer = false;
        }

        public override void SetBonus()
        {
            if (_target is PlayerTankScript player)
            {
                player.SetNewGas(150f);
            }
        }
    }
}
