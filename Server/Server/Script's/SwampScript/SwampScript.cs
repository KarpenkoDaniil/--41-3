using Behaivourus;
using GameObjects;
using OutputWindow.Scripts.TankScripts;

namespace Server.Script_s.SwampScript
{
    public class SwampScript : MonoBehaivour
    {
        public SwampScript(GameObject mesh) : base(mesh)
        {
            _copyScript = false;
        }

        private List<PlayerTankScript> playerTankScripts = new List<PlayerTankScript>();


        public override void Colision(object sender, EventArgs e)
        {
            if (sender is Collision colision)
            {
                foreach (var item in colision.gameObject.MonoBehaivours)
                {
                    if (item is PlayerTankScript player)
                    {
                        if (!player.PlayerAtSwamp() && !playerTankScripts.Contains(player))
                        {
                            player.AtSwamp();
                            lock (playerTankScripts)
                            {
                                if (!playerTankScripts.Contains(player))
                                    playerTankScripts.Add(player);
                            }
                        }
                    }
                }
            }
        }

        private List<PlayerTankScript> playerTanksDelete = new List<PlayerTankScript>();

        public override void Update(object sender, EventArgs e)
        {
            foreach (var item in playerTankScripts)
            {
                if (!gameObject.Collision.Collide(item.GameObject))
                {
                    item.OutSwamp();
                    playerTanksDelete.Add(item);
                }
            }

            foreach (var item in playerTanksDelete)
            {
                playerTankScripts.Remove(item);
            }
            playerTanksDelete.Clear();

        }
    }
}
