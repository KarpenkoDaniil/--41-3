using Behaivourus;
using GameLoader;
using GameObjects;
using OutputWindow.Network;

namespace Client.Game
{
    public class GameClient
    {
        bool gameStart = false;
        EventHandler _updateHandler;
        EventHandler _deleteHandler;
        EventHandler _startHandler;

        private List<GameObject> _gameObjects = new List<GameObject>();
        public List<GameObject> GameObjects => _gameObjects;

        private List<GameObject> _removeGameObjects = new List<GameObject>();


        public void GameState()
        {
            if (!gameStart)
            {
                _startHandler?.Invoke(this, null);
                gameStart = true;
            }
            SetStatet();
            _updateHandler?.Invoke(this, null);
            ClearObjects();
            TimeDeltaHelper.Update();
        }

        private Queue<List<StateObject>> _gameStatet = new Queue<List<StateObject>>();

        private void SetStatet()
        {
            if (_gameStatet.Count > 0)
            {
                List<StateObject> stateObjects = _gameStatet.Dequeue();

                GameObject gameObject;
                lock (_gameObjects)
                {
                    foreach (var state in stateObjects)
                    {
                        gameObject = GetObjectByID(state.ObjectID);
                        if (state.Type == StateObject.TypeOfMesage.Create)
                        {
                            if (gameObject == null)
                            {
                                gameObject = state.CreateObject();
                                AddGameObject(gameObject);
                            }
                        }
                        else if (state.Type == StateObject.TypeOfMesage.Read && gameObject != null)
                        {
                            state.SetStateObject(gameObject);
                        }
                        else if (state.Type == StateObject.TypeOfMesage.Delete && gameObject != null)
                        {
                            RemoveGameObject(gameObject);
                        }
                    }
                }
            }
        }

        public void UpdateState(List<StateObject> stateObjects)
        {
            lock (_gameObjects)
            {
                _gameStatet.Enqueue(stateObjects);
            }
        }

        public GameObject GetObjectByID(int ID)
        {
            foreach (var gameObject in _gameObjects)
            {
                if (gameObject.GameObjectID == ID)
                {
                    return gameObject;
                }
            }

            return null;
        }

        public GameObject GetGameObjectByName(string name)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                if (name == gameObject.NameObject)
                {
                    return gameObject;
                }
            }
            return null;
        }

        public void RemoveListeners(MonoBehaivour monoBehaivour)
        {
            _startHandler -= monoBehaivour.Start;
            _updateHandler -= monoBehaivour.Update;
        }

        //Добавление объекта в игровой цикл
        public void AddGameObject(GameObject gameObject)
        {
            lock (_gameObjects)
            {
                _gameObjects.Add(gameObject);
            }

            foreach (var item in gameObject.MonoBehaivours)
            {
                AddListenerToUpdate(item);
            }
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            lock (_removeGameObjects)
            {
                _removeGameObjects.Add(gameObject);
            }
        }

        public void AddListenerToUpdate(MonoBehaivour monoBehaivour)
        {
            _updateHandler += monoBehaivour.Update;
            _startHandler += monoBehaivour.Start;
        }

        public void ClearObjects()
        {
            lock (_removeGameObjects)
            {
                lock (_gameObjects)
                {
                    if (_removeGameObjects.Count > 0)
                    {
                        foreach (var gameObject in _removeGameObjects)
                        {
                            foreach (var monoBehaivour in gameObject.MonoBehaivours)
                            {
                                _updateHandler -= monoBehaivour.Update;
                                _startHandler -= monoBehaivour.Start;

                                gameObject.Collision.RemoveListener(monoBehaivour);
                                _gameObjects.Remove(gameObject);
                                monoBehaivour.Delete();
                            }
                        }
                        _removeGameObjects.Clear();
                    }
                }
            }
        }
    }
}
