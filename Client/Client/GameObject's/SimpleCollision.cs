using GameObjects;
using OpenTK;
using GameLoader;

namespace OutputWindow.GameObjects
{
    public class SimpleCollision
    {
        public bool IsTurn => _isTurn;
        private bool _isTurn = true;
        Vector3 _direction = Vector3.Zero;
        GameObject gameObjects;

        public void TurnOfOn()
        {
            _isTurn = !_isTurn;
        }

        public SimpleCollision(GameObject gameObjects)
        {
            this.gameObjects = gameObjects;
        }

        public void AddImpact(float veilocity, Vector3 direction)
        {
            Vector3 move = direction * veilocity * TimeDeltaHelper.DeltaT;
            _direction = direction;
            gameObjects.Mesh.MoveBy(move.X, move.Y, move.Z);
        }

        public void CorrectPosition(Vector3 selfMin, Vector3 selfMax, Vector3 wallMin, Vector3 wallMax)
        {
            if (_isTurn)
            {
                Vector3 correctedPosition = gameObjects.Position;

                // Проверка пересечения по X
                if (selfMax.X > wallMin.X && selfMin.X < wallMin.X && _direction.X > 0)
                {
                    // Двигаемся влево, отодвигаем вправо
                    correctedPosition.X = wallMin.X - (selfMax.X - selfMin.X) / 2;
                }
                else if (selfMin.X < wallMax.X && selfMax.X > wallMax.X && _direction.X < 0)
                {
                    // Двигаемся вправо, отодвигаем влево
                    correctedPosition.X = wallMax.X + (selfMax.X - selfMin.X) / 2;
                }

                // Проверка пересечения по Y
                if (selfMax.Y > wallMin.Y && selfMin.Y < wallMin.Y && _direction.Y > 0)
                {
                    // Двигаемся вверх, отодвигаем вниз
                    correctedPosition.Y = wallMin.Y - (selfMax.Y - selfMin.Y) / 2;
                }
                else if (selfMin.Y < wallMax.Y && selfMax.Y > wallMax.Y && _direction.Y < 0)
                {
                    // Двигаемся вниз, отодвигаем вверх
                    correctedPosition.Y = wallMax.Y + (selfMax.Y - selfMin.Y) / 2;
                }

                // Проверка пересечения по Z
                if (selfMax.Z > wallMin.Z && selfMin.Z < wallMin.Z && _direction.Z > 0)
                {
                    // Двигаемся вперёд, отодвигаем назад
                    correctedPosition.Z = wallMin.Z - (selfMax.Z - selfMin.Z) / 2;
                }
                else if (selfMin.Z < wallMax.Z && selfMax.Z > wallMax.Z && _direction.Z < 0)
                {
                    // Двигаемся назад, отодвигаем вперёд
                    correctedPosition.Z = wallMax.Z + (selfMax.Z - selfMin.Z) / 2;
                }

                gameObjects.Mesh.SetPosition(correctedPosition);
            }
        }
    }
}
