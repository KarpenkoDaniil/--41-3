using Behaivourus;
using GameObjects;
using NetWorkConnector;
using OutputWindow.Game;
using OutputWindow.Network;
using Server.MonoBehaivourus;
using Server.Network;

namespace Game
{
    public class GameEngine
    {
        List<GameObject> _gameObjects = new List<GameObject>();
        List<GameObject> _gameObjectsToDelete = new List<GameObject>();
        List<GameObject> _gameObjectsToAdd = new List<GameObject>();

        public List<GameObject> GameObjects => _gameObjects;

        EventHandler _handlerUpdate;
        EventHandler _handlerStart;
        public EventHandler DeleteEventOfObject;
        int threadCountMaxSize => 24;
        int threadCount = 0;
        bool _start = false;
        bool _objectLoaded = false;
        bool _restart = false;

        public GameEngine(ObjectLoader objectLoader)
        {
            TimeDeltaHelper.Start();
            SingeltonEngine.SetEngine(this);
            SingeltonEngine.SetObjectLoader(objectLoader);

            Thread thread = new Thread(WorkCycle);
            thread.Start();
        }

        public GameObject GetGameObjectByID(int ID)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                if (ID == gameObject.GameObjectID)
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

        public void AddObject(GameObject gameObject)
        {
            lock (_gameObjects)
            {
                _gameObjects.Add(gameObject);
            }

            foreach (var monoBehaivour in gameObject.MonoBehaivours)
            {
                _handlerUpdate += monoBehaivour.Update;
                _handlerStart += monoBehaivour.Start;
            }
        }

        public void RemoveListener(MonoBehaivour monoBehaivour)
        {
            _handlerUpdate -= monoBehaivour.Update;
            _handlerStart -= monoBehaivour.Start;
        }

        public void AddListener(MonoBehaivour monoBehaivour)
        {
            _handlerUpdate += monoBehaivour.Update;
            _handlerStart += monoBehaivour.Start;
        }

        private void AddListeners()
        {
            foreach (GameObject obj in _gameObjects)
            {
                foreach (var monoBehaivour in obj.MonoBehaivours)
                {
                    _handlerUpdate += monoBehaivour.Update;
                    _handlerStart += monoBehaivour.Start;
                }
            }
        }

        public void UpdateStatet()
        {
            var list = SingeltonEngine.ConectionManager.GetDataFromUsers();

            foreach (var gameState in list)
            {
                foreach (var mesage in gameState.GameStates)
                {
                    GameObject gameObject = GetGameObjectByID(mesage.ObjectID);
                    if (gameObject != null)
                    {
                        if (mesage.Type == StateObject.TypeOfMesage.Read)
                        {
                            mesage.ReturnObject(gameObject, gameState.Player.InputController);
                        }
                        else if (mesage.Type == StateObject.TypeOfMesage.Create)
                        {
                            mesage.CreateObject(gameState.Player.InputController);
                        }
                        else
                        {
                            SingeltonEngine.GameEngine.AddObjectToDelete(gameObject);
                        }
                    }
                }
            }
        }


        public void GetDataFromUser(object data, EventArgs eventArgs)
        {
            var gameState = (GameState)data;

            foreach (var mesage in gameState.GameStates)
            {
                GameObject gameObject = GetGameObjectByID(mesage.ObjectID);
                if (gameObject != null)
                {
                    if (mesage.Type == StateObject.TypeOfMesage.Read)
                    {
                        mesage.ReturnObject(gameObject, gameState.Player.InputController);
                    }
                    else if (mesage.Type == StateObject.TypeOfMesage.Create)
                    {
                        mesage.CreateObject(gameState.Player.InputController);
                    }
                    else
                    {
                        SingeltonEngine.GameEngine.AddObjectToDelete(gameObject);
                    }
                }
            }
        }

        public void Restart()
        {
            foreach (GameObject obj in _gameObjects)
            {
                SingeltonEngine.ConectionManager.AddObjectToSend(obj, StateObject.TypeOfMesage.Delete);
                AddObjectToDelete(obj);
            }
            SingeltonEngine.ConectionManager.SendStateFromServer();
            ClearObjects();

            _restart = true;
            _objectLoaded = false;
            _start = false;
        }

        public void Start()
        {
            if (!_objectLoaded)
            {
                var gmOBJ = SingeltonEngine.ObjectLoader.Load();
                foreach (var item in gmOBJ)
                {
                    AddObject(item);
                }
                _objectLoaded = true;
            }

            _start = false;
        }

        int FPS_COUNTER = 0;
        float _time = 0;

        private object _monitor = new object();
        SemaphoreSlim _semaphore = new SemaphoreSlim(24);

        private const float TARGET_FPS = 120;
        private const float FRAME_TIME = 1000 / TARGET_FPS; // миллисекунды на кадр

        //-------------------------------------------------
        private async void WorkCycle()
        {
            while (true)
            {
                var startTime = DateTime.Now;
                var newColGameObj = new List<GameObject>(_gameObjects);
                List<Task> tasks = new List<Task>();
                int count = 0;
                int cicle_count = 0;

                UpdateStatet();

                // Обработка коллизий
                foreach (var gameObject in newColGameObj)
                {
                    gameObject.Collision.MoveCollision();
                    

                    if (gameObject.Collision.isTurn && !gameObject.IsDestroyed && !gameObject.isStatic)
                    {
                        var task = ProcessCollisionAsync(gameObject, newColGameObj, cicle_count + 1);
                        tasks.Add(task);
                        count++;
                    }
                    cicle_count++;
                }

                // Ожидаем завершения всех задач проверки коллизий
                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                }

                _handlerUpdate?.Invoke(this, null);

                //Сигнал на отправку пакета
                SingeltonEngine.ConectionManager.SendStateFromServer();
                SingeltonEngine.ConectionManager.EndOfState();
                if (!_start)
                {
                    Start();
                    _handlerStart.Invoke(this, null);
                    SingeltonEngine.ConectionManager.SetPlaybelObjects();
                    if (_restart)
                    {
                        SingeltonEngine.ConectionManager.SendDataOnRestart();
                        _restart = false;
                    }
                    _start = true;
                }
                
                ClearObjects();
                
                if (_time < 1.0)
                {
                    _time = _time + TimeDeltaHelper.DeltaT;
                    FPS_COUNTER++;
                }
                else
                {
                    //Console.WriteLine("COUNT FPS " + FPS_COUNTER);
                    //Console.WriteLine("COUNT " + count + " COUNT OBJ " + newColGameObj.Count + " CYCLE COUNT " + cicle_count);
                    FPS_COUNTER = 0;
                    _time = 0;
                }

                var processingTime = (DateTime.Now - startTime).TotalMilliseconds;
                var sleepTime = FRAME_TIME - processingTime;

                TimeDeltaHelper.Update();
            }
        }

        private async Task ProcessCollisionAsync(GameObject gameObject, List<GameObject> colList, int index_start)
        {
            await _semaphore.WaitAsync();
            try
            {
                await Task.Run(() => CheckCol(gameObject, colList, index_start));
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private void CheckCol(GameObject gameObject, List<GameObject> gameObjects, int index_start)
        {
            foreach (var nextObject in gameObjects)
            {
                if (nextObject.Collision.isTurn && nextObject.GameObjectID != gameObject.GameObjectID)
                {
                    nextObject.Collision.MoveCollision();
                    gameObject.Collision.CheckCollision(nextObject);
                }
            }
            threadCount--;
        }

        private void ClearObjects()
        {
            if (_gameObjectsToDelete.Count > 0)
            {
                foreach (var gameObject in _gameObjectsToDelete)
                {
                    foreach (var monoBehaivour in gameObject.MonoBehaivours)
                    {
                        _handlerUpdate -= monoBehaivour.Update;
                        _handlerStart -= monoBehaivour.Start;
                        gameObject.Collision.RemoveListener(monoBehaivour);
                        lock (_gameObjects)
                        {
                            _gameObjects.Remove(gameObject);
                        }
                        DeleteEventOfObject?.Invoke(gameObject, null);
                    }
                }
                _gameObjectsToDelete.Clear();
            }
        }

        public void AddObjectToDelete(GameObject gameObject)
        {
            lock (_gameObjectsToDelete)
            {
                _gameObjectsToDelete.Add(gameObject);
            }
        }
    }
}
