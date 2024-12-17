using Behaivourus;
using GameObjects;
using OutputWindow.Scripts.TankScripts;
using Server.GameObject.Interface;

namespace Client.Scripts.UI_Script
{
    public class UIScript : MonoBehaivour
    {
        private UserInterface2D _userInterface2D;
        private PlayerTankScript _playerTankScript;
        public UIScript(GameObject mesh) : base(mesh)
        {
        }

        public void SetUserInterface(UserInterface2D userInterface2D, PlayerTankScript playerTankScript)
        {
            _userInterface2D = userInterface2D;
            _playerTankScript = playerTankScript;
        }

        public override void Update(object sender, EventArgs e)
        {
            _userInterface2D.Text = "";
            _userInterface2D.Text = "HP: " + _playerTankScript.CurrentHealth + "\n";
            _userInterface2D.Text = _userInterface2D.Text + "Fuel Gas: " + _playerTankScript.CurentGas + "\n";
        }
    }
}
