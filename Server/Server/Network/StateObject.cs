using GameObjects;
using OutputWindow.Game;
using Server.GameLogic.InputController;
using Server.MonoBehaivourus;


namespace OutputWindow.Network
{
    public class StateObject
    {
        public enum TypeOfMesage
        { Create, Read, Delete }

        public TypeOfMesage Type;

        public string TypeOfObject;
        public string NameOfGameObject;
        public string NameOfTexture;
        public bool isAlive = true;
        public int ObjectID;

        public float Roll;
        public float Yaw;
        public float Pitch;

        public float[] Position;
        public float Scale;
        public int TextureID;
        public bool Visible;
        public int Layer;

        public List<StateScript> Behaivours = new List<StateScript>();

        public StateObject()
        {
        }

        public StateObject(GameObject gameObject, TypeOfMesage type)
        {
            this.Type = type;

            TypeOfObject = gameObject.Texture.Name.Split(".")[0];
            NameOfGameObject = gameObject.NameObject;
            NameOfTexture = gameObject.Texture.Name;
            TextureID = gameObject.Texture.Index;
            ObjectID = gameObject.GameObjectID;
            Roll = gameObject.Mesh.Roll;
            Yaw = gameObject.Mesh.Yaw;
            Pitch = gameObject.Mesh.Pitch;

            Position = new float[3];
            Position[0] = gameObject.Position.X;
            Position[1] = gameObject.Position.Y;
            Position[2] = gameObject.Position.Z;
            Visible = gameObject.IsVisible;
            Layer = gameObject.Mesh.Layer;

            Scale = gameObject.Mesh.Scale;
            foreach (var item in gameObject.MonoBehaivours)
            {
                if (item.IsCopingScript())
                {
                    Behaivours.Add(new StateScript(item));
                }
            }
        }

        //Для обновления объекта
        public void ReturnObject(GameObject gameObject, PlayerInputController playerInputController)
        {
            if (TypeOfMesage.Create == Type)
                gameObject.SetTexture(SingeltonEngine.ObjectLoader.GetTexturByID(TextureID));

            int index = 0;
            foreach (var item in gameObject.MonoBehaivours)
            {
                if (item.IsCopingScript())
                {
                    if (item is OnlinePlaybleBehaivour online)
                    {
                        if (online.GetToken() == playerInputController.PlayerToken)
                        {
                            item.SateState(Behaivours[index].StatementOfScript, playerInputController, false);
                        }
                    }
                    else
                    {
                        item.SateState(Behaivours[index].StatementOfScript, playerInputController);
                    }
                }
                index++;
            }
        }

        public void CreateObject(PlayerInputController playerInputController)
        {
            GameObject gameObject = SingeltonEngine.ObjectLoader.CreateObject(NameOfGameObject);
            gameObject.NameObject = NameOfGameObject;
            gameObject.GameObjectID = ObjectID;
            gameObject.Mesh.SetPosition(new OpenTK.Vector3(Position[0], Position[1], Position[2]));

            gameObject.Mesh.Roll = Roll;
            gameObject.Mesh.Yaw = Yaw;
            gameObject.Mesh.Pitch = Pitch;

            gameObject.Mesh.Scale = Scale;
            gameObject.IsVisible = Visible;
            gameObject.Mesh.Layer = Layer;

            if (TypeOfMesage.Create == Type)
                gameObject.SetTexture(SingeltonEngine.ObjectLoader.GetTexturByID(TextureID));

            int index = 0;
            foreach (var item in gameObject.MonoBehaivours)
            {
                item.SateState(Behaivours[index].StatementOfScript, playerInputController, false);
                index++;
            }
            SingeltonEngine.GameEngine.AddObject(gameObject);
        }
    }
}
