using Behaivourus;
using OpenTK;

namespace GameObjects
{
    public class Collision
    {
        public bool Detected = false;
        private GameObject _gameObject;
        public GameObject gameObject => _gameObject;

        private Vector3 _min;
        private Vector3 _max;

        public Vector3 Min => _min;
        public Vector3 Max => _max;

        private EventHandler _colider;
        public EventHandler Colider => _colider;
        private EventHandler _triger;
        public EventHandler Trigger => _triger;

        public bool isTurn = true;
        public bool isTriger = false;

        public Collision(GameObject gameObject)
        {
            _gameObject = gameObject;
            FoundMaxPos();
            FoundMinPos();
            AddListener();
        }

        private void FoundMinPos()
        {
            List<Vector3> ver = new List<Vector3>();
            foreach (var item in gameObject.Mesh.Vercites)
            {
                ver.Add(Vector3.TransformPosition(item, gameObject.Matrix));
            }

            var firstPos = ver[0];
            Vector3 minPos = new Vector3(firstPos.X, firstPos.Y, firstPos.Z);

            foreach (var pos in ver)
            {
                if (minPos.X > pos.X)
                {
                    minPos.X = pos.X;
                }
                else if (minPos.Y > pos.Y)
                {
                    minPos.Y = pos.Y;
                }
                else if (minPos.Z > pos.Z)
                {
                    minPos.Z = pos.Z;
                }
            }
            _min = minPos;
        }

        private void FoundMaxPos()
        {
            List<Vector3> ver = new List<Vector3>();
            foreach (var item in gameObject.Mesh.Vercites)
            {
                ver.Add(Vector3.TransformPosition(item, gameObject.Matrix));
            }

            var firstPos = ver[0];
            Vector3 maxPos = new Vector3(firstPos.X, firstPos.Y, firstPos.Z);

            foreach (var pos in ver)
            {
                if (maxPos.X < pos.X)
                {
                    maxPos.X = pos.X;
                }
                else if (maxPos.Y < pos.Y)
                {
                    maxPos.Y = pos.Y;
                }
                else if (maxPos.Z < pos.Z)
                {
                    maxPos.Z = pos.Z;
                }
            }

            _max = maxPos;
        }

        public void MoveCollision()
        {
            FoundMaxPos();
            FoundMinPos();
        }

        public void AddListener(MonoBehaivour monoBehaivour)
        {
            _colider += monoBehaivour.Colision;
            _triger += monoBehaivour.Colision;
        }

        private void AddListener()
        {
            foreach (var behaivour in this.gameObject.MonoBehaivours)
            {
                _colider += behaivour.Colision;
                _triger += behaivour.Trigger;
            }
        }

        public void RemoveListener(MonoBehaivour monoBehaivour)
        {
            _colider -= monoBehaivour.Colision;
            _triger -= monoBehaivour.Colision;
        }

        public void RemoveListener()
        {
            foreach (var monoBehaivour in _gameObject.MonoBehaivours)
            {
                _colider -= monoBehaivour.Colision;
                _triger -= monoBehaivour.Colision;
            }
        }

        public void CheckCollision(GameObject gameObject)
        {
            Vector3 selfMin = _min;
            Vector3 selMax = _max;

            Vector3 min = gameObject.Collision.Min;
            Vector3 max = gameObject.Collision.Max;

            bool isIntersect = Intersect(min, max, selfMin, selMax);

            if (isIntersect)
            {
                if (_gameObject.SimpleCollision.IsTurn && !_gameObject.Collision.isTriger && !gameObject.Collision.isTriger)
                {
                    _gameObject.SimpleCollision.CorrectPosition(selfMin, selMax, min, max);
                }
                this.Invoke(gameObject);
                Detected = true;
            }
        }

        public bool Collide(GameObject gameObject)
        {
            Vector3 selfMin = _min;
            Vector3 selMax = _max;

            Vector3 min = gameObject.Collision.Min;
            Vector3 max = gameObject.Collision.Max;

            bool isIntersect = Intersect(min, max, selfMin, selMax);

            return isIntersect;
        }

        private bool Intersect(Vector3 min, Vector3 max, Vector3 selfMin, Vector3 selfMax)
        {
            return selfMin.X < max.X && selfMax.X > min.X &&
                selfMin.Y < max.Y && selfMax.Y > min.Y;
        }

        private void Invoke(GameObject gameObject)
        {
            if (!isTriger)
            {
                this._colider?.Invoke(gameObject.Collision, null);
            }
            else
            {
                this._triger?.Invoke(gameObject.Collision, null);
            }

            if (!gameObject.Collision.isTriger)
            {
                gameObject.Collision.Colider?.Invoke(gameObject.Collision, null);
            }
            else
            {
                gameObject.Collision.Trigger?.Invoke(gameObject.Collision, null);
            }
        }
    }
}
