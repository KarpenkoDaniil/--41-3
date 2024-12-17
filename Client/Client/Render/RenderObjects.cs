using GameLoader;
using GameObjects;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Server.GameObject.Interface;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace RenderObject
{
    public class RenderObjects
    {
        List<UserInterface2D> _userInterface2D = new List<UserInterface2D>();
        GLControl _glControl;
        Form _form;
        int[] layers = new int[3] {0, 1, 2};
        public RenderObjects(GLControl gLControl, Form form)
        {
            _glControl = gLControl;
            _form = form;

            _glControl.Paint += RenderCicle;
            _glControl.Load += Control_Resize;
            _glControl.Resize += Control_Resize;
            _glControl.Dock = DockStyle.Fill;
            _form.Controls.Add(_glControl);
        }

        public void RestartRender()
        {
            lock (_userInterface2D)
            {
                _userInterface2D.Clear();
            }
        }

        public void AddToRender2DUI(UserInterface2D interface2D)
        {
            lock (_userInterface2D)
            {
                _userInterface2D.Add(interface2D);
            }
        }

        public void RemoveFromRender2DUI(UserInterface2D userInterface2D)
        {
            lock (_userInterface2D)
            {
                _userInterface2D.Remove(userInterface2D);
            }
        }

        private List<GameObject> _renderedObjects = new List<GameObject>();

        public void RenderCicle(object sender, PaintEventArgs eventPaint)
        {
            _glControl.MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1);

            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, _form.Width, _form.Height, 0, 1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            var curDraw = new List<GameObject>(SingeltonEngine.GameClient.GameObjects);
            int count = curDraw.Count;
            foreach (int layer in layers)
            {
                foreach (GameObject gameObject in curDraw)
                {
                    if (gameObject != null)
                    if (gameObject.IsVisible)
                    {
                        if (layer == gameObject.Mesh.Layer && !_renderedObjects.Contains(gameObject))
                        {
                            _renderedObjects.Add(gameObject);
                            RenderObject(gameObject);
                        }
                    }
                }
            }

            _renderedObjects.Clear();

            GL.PopMatrix();

            lock (_userInterface2D)
            {
                foreach (var UI in _userInterface2D)
                {
                    RenderUI2D(UI);
                }
            }

            _glControl.SwapBuffers();
        }

        private void RenderUI2D(UserInterface2D userInterface2D)
        {
            Font font = new Font("Arial", 14);
            int textureId;

            // Сохраняем текущую матрицу
            GL.PushMatrix();

            //// Переключаемся в режим 2D
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, _form.Width, _form.Height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            // Создаем текстуру
            GL.GenTextures(1, out textureId);

            // Измеряем размер текста
            Bitmap tempBitmap = new Bitmap(1, 1);
            Graphics tempGraphics = Graphics.FromImage(tempBitmap);
            SizeF textSize = tempGraphics.MeasureString(userInterface2D.Text, font);
            tempGraphics.Dispose();
            tempBitmap.Dispose();

            // Создаем текстуру с текстом
            int textureWidth = (int)Math.Ceiling(textSize.Width);
            int textureHeight = (int)Math.Ceiling(textSize.Height);

            UpdateTextTexture(userInterface2D.Text, font, textureId, textureWidth, textureHeight);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.BindTexture(TextureTarget.Texture2D, textureId);

            float x = userInterface2D.X_Position;
            float y = userInterface2D.Y_Position;

            GL.Begin(PrimitiveType.Quads);
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); // Устанавливаем белый цвет с полной непрозрачностью
            GL.TexCoord2(0, 0); GL.Vertex2(x, y);
            GL.TexCoord2(1, 0); GL.Vertex2(x + textureWidth, y);
            GL.TexCoord2(1, 1); GL.Vertex2(x + textureWidth, y + textureHeight);
            GL.TexCoord2(0, 1); GL.Vertex2(x, y + textureHeight);
            GL.End();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);

            GL.PopMatrix();
            GL.DeleteTexture(textureId);
        }

        private void UpdateTextTexture(string text, Font font, int textureId, int width, int height)
        {
            // Создаем bitmap с текстом
            if (width != 0 && height != 0)
                using (Bitmap bitmap = new Bitmap(width, height))
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Transparent);
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(text, font, Brushes.Aqua, new PointF(0, 0));

                    BitmapData data = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.BindTexture(TextureTarget.Texture2D, textureId);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                        data.Width, data.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, data.Scan0);

                    bitmap.UnlockBits(data);

                    GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                    GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
                }
        }

        private void RenderObject(GameObject gameObject)
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, gameObject.Texture.GetWidth(), gameObject.Texture.GetHeight(),
                0, PixelFormat.Bgra, PixelType.UnsignedByte, gameObject.Texture.GetData());

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc((BlendingFactor)BlendingFactorSrc.SrcAlpha, (BlendingFactor)BlendingFactorDest.OneMinusSrcAlpha);

            GL.Begin(PrimitiveType.Quads);

            Matrix4 transformMT = gameObject.Matrix;
            for (int i = 0; i < gameObject.Mesh.Vercites.Length / 1; i++)
            {
                Vector3 vercite = Vector3.TransformPosition(gameObject.Mesh.Vercites[i], transformMT);
                GL.Vertex3(vercite);
                GL.TexCoord2(gameObject.Texture.TextureCords[i]);
            }

            GL.End();

            GL.DeleteTexture(texture);

            GL.Begin(PrimitiveType.Lines); // Начало отрисовки линии

            if (gameObject.Collision.Detected)
            {
                var verctMax = Vector3.TransformPosition(gameObject.Collision.Max, gameObject.Matrix);
                var verctMin = Vector3.TransformPosition(gameObject.Collision.Min, gameObject.Matrix);

                GL.Color3(1.0f, 1.0f, 1.0f); // Установка цвета линии (красный)
                GL.Vertex2(verctMax.X, verctMax.Y); // Начальная точка линии
                GL.Vertex2(verctMin.X, verctMin.Y); // Конечная точка линии
            }

            GL.End();
        }

        // Расположение окна
        private void ScreenPosition()
        {
            // Получаем размеры экрана
            Screen screen = Screen.PrimaryScreen;
            double screenWidth = screen.Bounds.Width;
            double screenHeight = screen.Bounds.Height;

            // Получаем размеры окна
            double windowWidth = _form.Width;
            double windowHeight = _form.Height;

            // Вычисляем координаты окна
            double x = (screenWidth - windowWidth) / 2;
            double y = (screenHeight - windowHeight) / 2;

            // Устанавливаем новые координаты окна
            _form.Left = (int)x;
            _form.Top = (int)y;
        }

        // отвечает за размер объектов от размеров окна 
        private void Control_Resize(object sender, EventArgs e)
        {
            if (_glControl.Width <= 1080)
            {
                _form.Width = 600;
                _form.Height = 600;
                GL.Viewport(0, 0, _glControl.Width, _glControl.Height);

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();

                GL.Ortho(-1, 1, -1, 1, -1, 1);
                GL.MatrixMode(MatrixMode.Modelview);

                GL.LoadIdentity();

            }
            else
            {
                GL.Viewport(_glControl.Width / 4, 0, 1080, 1020);

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();

                GL.Ortho(-1, 1, -1, 1, -1, 1);
                GL.MatrixMode(MatrixMode.Modelview);

                GL.LoadIdentity();
            }
        }

        public void UpdateScreen()
        {
            _glControl.Invalidate();
        }

        public void OnOfRender(bool isWork)
        {
            _glControl.Visible = isWork;
        }
    }
}
