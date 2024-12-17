using GameObjects;
using OutputWindow.Scripts.TankScripts;

namespace OutputWindow.Scripts.BonusScript
{
    public class ArmorBehaivour : BonusBehaivour
    {
        public ArmorBehaivour(GameObject mesh) : base(mesh)
        {
        }

        public override void SetBonus()
        {
            _hasTimer = false;
            if (_target is PlayerTankScript player)
            {
                player.SetDeltaArmmor(10f);
            }
        }

        public override void RemoveBonus()
        {
            //if (_target is PlayerTankScript player)
            //{
            //    player.SetDeltaArmmor(10f);
            //}
        }
    }
}
