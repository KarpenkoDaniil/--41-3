using OpenTK;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GameObjects
{
    public class Texture
    {
        public int Handle;
        public string Name => name;

        private Vector2[] _textureCords;
        public Vector2[] TextureCords => _textureCords;

        public int Index;

        byte[] data;
        int height;
        int width;
        string name;
        // чтение растрого изображения 
        public Texture(string path, int index)
        {
            Index = index;
            string[] folders = path.Split("\\");
            using (Bitmap bitmap = new Bitmap(path))
            {
                name = folders[folders.Length - 1].Split(".")[0];
                height = bitmap.Height;
                width = bitmap.Width;
                BitmapData wallData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                IntPtr intPtr = wallData.Scan0;
                int bytes = wallData.Stride * wallData.Height;
                data = new byte[bytes];
                Marshal.Copy(intPtr, data, 0, bytes);

                bitmap.UnlockBits(wallData);
                bitmap.Dispose();

                _textureCords = new Vector2[4];

                _textureCords[0] = new Vector2(1f, 1f);
                _textureCords[1] = new Vector2(1f, 0f);
                _textureCords[2] = new Vector2(0f, 0f);
                _textureCords[3] = new Vector2(0f, 1f);
            }
        }
        // получения массива байт с текстурой
        public byte[] GetData() { return data; }
        // получение высоты
        public int GetHeight() { return height; }
        // получение длинны
        public int GetWidth() { return width; }
    }
}
