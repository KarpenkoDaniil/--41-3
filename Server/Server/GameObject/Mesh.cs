using Newtonsoft.Json;
using OpenTK;

namespace GameObjects
{
    public class Mesh
    {
        [JsonProperty] private float _scale = 1f;
        public float Scale { get => _scale; set => _scale = value; }

        private float _scaleX = 1f;
        private float _scaleY = 1f;
        private float _scaleZ = 1f;

        [JsonProperty] private Vector3 _position;
        public Vector3 Position { get => _position; set => _position = value; }

        [JsonProperty] private float _yaw;
        public float Yaw { get => _yaw; set => _yaw = value; }

        [JsonProperty] private float _pitch;
        public float Pitch { get => _pitch; set => _pitch = value; }

        [JsonProperty] private float _roll;
        public float Roll { get => _roll; set => _roll = value; }

        [JsonProperty] private Vector3[] _vercites;
        public Vector3[] Vercites => _vercites;

        public int Layer;

        public Mesh(Vector3 position, Vector3[] vercites, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f)
        {
            _position = position;
            _vercites = vercites;
            _yaw = yaw;
            _pitch = pitch;
            _roll = roll;
        }

        public void SetPosition(Vector3 pos)
        {
            _position = pos;
        }

        private void LimitAngleByPlusMinusPi(ref float angle)
        {
            if (angle > MathHelper.Pi) angle -= MathHelper.TwoPi;
            else if (angle < -MathHelper.Pi) angle += MathHelper.TwoPi;
        }

        public virtual void YawBy(float deltaYaw)
        {
            _yaw += deltaYaw;
            LimitAngleByPlusMinusPi(ref _yaw);
        }

        public virtual void PitchBy(float deltaPitch)
        {
            _pitch += deltaPitch;
            LimitAngleByPlusMinusPi(ref _pitch);
        }

        public virtual void RollBy(float deltaRoll)
        {
            _roll += deltaRoll;
            LimitAngleByPlusMinusPi(ref _roll);
        }

        public virtual void MoveBy(float deltaX, float deltaY, float deltaZ)
        {
            _position.X += deltaX;
            _position.Y += deltaY;
            _position.Z += deltaZ;
        }

        public virtual void MoveBy(Vector4 direction)
        {
            _position.X += direction.X;
            _position.Y += direction.Y;
            _position.Z += direction.Z;
        }

        public virtual void MoveTo(float x, float y, float z)
        {
            _position.X = x;
            _position.Y = y;
            _position.Z = z;
        }

        public virtual void ScaleTo(float scale)
        {
            _scale = scale;
            _scaleX = scale;
            _scaleY = scale;
            _scaleZ = scale;
        }

        public void ScaleOnX_Axis(float scale)
        {
            _scale = scale;
            _scaleX = scale;
            _scaleY = scale;
            _scaleZ = scale;
        }

        public void ScaleOnY_Axis(float scale)
        {
            _scaleY = scale;
        }

        public void ScaleOnZ_Axis(float scale)
        {
            _scaleZ = scale;
        }

        public void SetSclae(Vector3 vector)
        {
            _scaleX = vector.X;
            _scaleY = vector.Y;
            _scaleZ = vector.Z;
        }

        public Matrix4 GetWorldMatrix()
        {
            return Matrix4.Identity *
                    Matrix4.CreateScale(new Vector3(_scaleX, _scaleY, _scaleZ)) *
                    Matrix4.CreateRotationY(_yaw) *
                    Matrix4.CreateRotationX(_pitch) *
                    Matrix4.CreateRotationZ(_roll) *
                    Matrix4.CreateTranslation(_position);
        }
    }
}
