using GameObjects;
using GameLoader;
using OpenTK;

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
            Visible = gameObject.IsVisible;
            Layer = gameObject.Mesh.Layer;

            Position = new float[3];
            Position[0] = gameObject.Position.X;
            Position[1] = gameObject.Position.Y;
            Position[2] = gameObject.Position.Z;
            
            Scale = gameObject.Mesh.Scale;
            foreach (var item in gameObject.MonoBehaivours)
            {
                Behaivours.Add(new StateScript(item));
            }
        }

        //Для обновления объекта
        public void SetStateObject(GameObject gameObject)
        {
            gameObject.NameObject = NameOfGameObject;
            gameObject.GameObjectID = ObjectID;
            gameObject.Mesh.SetPosition(new Vector3(Position[0], Position[1], Position[2]));

            gameObject.Mesh.Roll = Roll;
            gameObject.Mesh.Yaw = Yaw;
            gameObject.Mesh.Pitch = Pitch;

            gameObject.Mesh.ScaleTo(Scale);
            gameObject.IsVisible = Visible;
            gameObject.Mesh.Layer = Layer;

            int index = 0;
            foreach (var item in gameObject.MonoBehaivours)
            {
                item.SateState(Behaivours[index].StatementOfScript);
                index++;
            }
        }

        public GameObject CreateObject()
        {
            GameObject gameObject = SingeltonEngine.ObjectLoader.CreateObject(NameOfGameObject, Behaivours);
            gameObject.NameObject = NameOfGameObject;
            gameObject.GameObjectID = ObjectID;
            gameObject.Mesh.SetPosition(new Vector3(Position[0], Position[1], Position[2]));

            gameObject.Mesh.Roll = Roll;
            gameObject.Mesh.Yaw = Yaw;
            gameObject.Mesh.Pitch = Pitch;

            gameObject.Mesh.ScaleTo(Scale);
            gameObject.IsVisible = Visible;
            gameObject.Mesh.Layer = Layer;

            if (TypeOfMesage.Create == Type)
                gameObject.SetTexture(SingeltonEngine.ObjectLoader.GetTexturByID(TextureID));

            int index = 0;
            foreach (var item in gameObject.MonoBehaivours)
            {
                item.SateState(Behaivours[index].StatementOfScript);
                index++;
            }

            return gameObject;
        }
    }
}
