using Behaivourus;

namespace OutputWindow.Network
{
    public class StateScript
    {
        public int ObjectID;
        public List<object> StatementOfScript;
        public string TypeOfScript;

        public StateScript()
        {

        }

        public StateScript(MonoBehaivour monoBehaivour)
        {
            ObjectID = monoBehaivour.GameObject.GameObjectID;
            StatementOfScript = monoBehaivour.ReturnSatate(monoBehaivour.GetType());
            TypeOfScript = monoBehaivour.GetType().ToString();
        }
    }
}
