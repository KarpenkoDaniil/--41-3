using IputController;
using NetWorkConnector;
using OpenTK;
using GameLoader;
using RenderObject;
using System.Net;
using Client.Game;

namespace Client
{
    public partial class TankBattle : Form
    {
        GLControl _glControl;
        RenderObjects _render;
        //GameEngine _engine;
        ConectionManager _conectionManager;

        Panel _mainMenu;
        Panel _secondMenu;
        Panel _thirdMenu;

        public bool IsHost = true;

        public bool Stop = false;

        public TankBattle()
        {
            InitializeComponent();
            this.Name = "Tank Battle";
            object renderMonitor = new object();
            _glControl = new GLControl();

            InputControlller.InitInputConroller(_glControl);
            _render = new RenderObjects(_glControl, this);

            LoaderTexture loaderTexture = new LoaderTexture();
            loaderTexture.LoadTextures();

            string pathToMap = AppDomain.CurrentDomain.BaseDirectory + "MetaData\\Map's\\Map1.bmp";
            ObjectLoader objectLoader = new ObjectLoader(loaderTexture.Textures);

            GameClient gameClient = new GameClient();

            _conectionManager = new ConectionManager();
            SingeltonEngine.SetConectionMenager(_conectionManager);
            SingeltonEngine.SetRender(_render);
            SingeltonEngine.SetObjectLoader(objectLoader);
            SingeltonEngine.SetLoaderTextures(loaderTexture);
            SingeltonEngine.SetGameClient(gameClient);

            _glControl.Visible = false;
            InitMenu();
            InitSecondMenu();
            InitThirdMenu();
        }

        public Panel InitMenu()
        {
            _mainMenu = new Panel();

            var buttonStart = new Button();
            buttonStart.Text = "Start Game";
            buttonStart.Name = "StartGameButton";
            buttonStart.Size = new Size(100, 40);
            buttonStart.Location = new Point(
                (this.Width - buttonStart.Width) / 2,
                (this.Height / 2) - buttonStart.Height / 2 + this.Height / 10
            );
            buttonStart.Click += ButtonStart_Click;

            var buttonEnd = new Button();
            buttonEnd.Text = "Exit";
            buttonEnd.Name = "ExitButton";
            buttonEnd.Size = new Size(100, 40);
            buttonEnd.Location = new Point(
                (this.Width - buttonEnd.Width) / 2,
                (this.Height / 2) - buttonEnd.Height / 2 - this.Height / 10
            );
            buttonEnd.Click += ButtonEnd_Click;

            _mainMenu.Dock = DockStyle.Fill;
            _mainMenu.Controls.Add(buttonStart);
            _mainMenu.Controls.Add(buttonEnd);
            this.Controls.Add(_mainMenu);

            return _mainMenu;
        }

        public Panel InitSecondMenu()
        {
            _secondMenu = new Panel();

            var buttonNotHost = new Button();
            buttonNotHost.Text = "Not a host";
            buttonNotHost.Name = "INotHOSTButton";
            buttonNotHost.Size = new Size(100, 40);
            buttonNotHost.Location = new Point(
                (this.Width - buttonNotHost.Width) / 2,
                (this.Height / 2) - buttonNotHost.Height / 2 - this.Height / 10
            );
            buttonNotHost.Click += ButtonNotHost_Click;

            _secondMenu.Dock = DockStyle.Fill;
            _secondMenu.Controls.Add(buttonNotHost);
            _secondMenu.Visible = false;
            this.Controls.Add(_secondMenu);

            return _secondMenu;
        }

        public Panel InitThirdMenu()
        {
            _thirdMenu = new Panel();

            Label labelIP = new Label();
            labelIP.Text = "Input IP Address";
            labelIP.AutoSize = true;
            labelIP.Location = new Point((this.Width - labelIP.Width) / 2, (this.Height / 2) - this.Height / 5);

            _textBoxIP = new TextBox();
            _textBoxIP.Size = new Size(120, 25);
            _textBoxIP.Location = new Point((this.Width - _textBoxIP.Width) / 2, (this.Height / 2) - this.Height / 10);
            _textBoxIP.Text = "127.0.0.1";

            Label labelPORT = new Label();
            labelPORT.Text = "Input PORT";
            labelPORT.AutoSize = true;
            labelPORT.Location = new Point((this.Width - labelPORT.Width) / 2, (this.Height / 2));

            _textBoxPort = new TextBox();
            _textBoxPort.Size = new Size(120, 25);
            _textBoxPort.Location = new Point((this.Width - _textBoxPort.Width) / 2, (this.Height / 2) + this.Height / 10);
            _textBoxPort.Text = "2000";

            _button = new Button();
            _button.Text = "Connect to Server";
            _button.Size = new Size(100, 40);
            _button.Location = new Point((this.Width - _button.Width) / 2, (this.Height / 2) + this.Height / 5);

            _thirdMenu.Dock = DockStyle.Fill;
            _thirdMenu.Controls.Add(_button);
            _thirdMenu.Controls.Add(labelIP);
            _thirdMenu.Controls.Add(labelPORT);
            _thirdMenu.Controls.Add(_textBoxIP);
            _thirdMenu.Controls.Add(_textBoxPort);

            this.Controls.Add(_thirdMenu);

            return _thirdMenu;
        }

        private void ButtonEnd_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        TextBox _textBoxIP;
        TextBox _textBoxPort;
        Button _button;

        string IP => _textBoxIP.Text;
        string PORT => _textBoxPort.Text;

        private void Button_Click_ConnectToHost(object? sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(IP);
            IPEndPoint iPEndPoint = new IPEndPoint(ip, int.Parse(PORT));
            SingeltonEngine.ConectionManager.SetConnection(iPEndPoint);
            _glControl.Visible = true;
        }

        private void ButtonNotHost_Click(object? sender, EventArgs e)
        {
            IsHost = false;
            _mainMenu.Visible = false;
            _secondMenu.Visible = false;
            _thirdMenu.Visible = true;
            _button.Text = "Connect";
            _button.Click += Button_Click_ConnectToHost;
        }

        private void ButtonStart_Click(object? sender, EventArgs e)
        {
            _mainMenu.Visible = false;
            _secondMenu.Visible = true;
            _thirdMenu.Visible = false;
        }
    }
}
