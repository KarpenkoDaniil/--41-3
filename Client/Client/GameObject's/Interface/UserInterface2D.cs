using GameLoader;

namespace Server.GameObject.Interface
{
    public class UserInterface2D
    {
        public string Text = "";
        public int X_Position = 0;
        public int Y_Position = 0;

        public UserInterface2D()
        {
            SingeltonEngine.Render.AddToRender2DUI(this);
        }
    }
}
