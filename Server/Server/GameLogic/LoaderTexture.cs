﻿using GameObjects;

namespace Game
{
    public class LoaderTexture
    {
        public List<Texture> Textures => _textures;
        private List<Texture> _textures = new List<Texture>();

        public void LoadTextures()
        {
            string mainDir = AppDomain.CurrentDomain.BaseDirectory + "MetaData\\Texture's\\";

            string[] files = Directory.GetFiles(mainDir);

            int index = 0;

            foreach (string file in files)
            {
                Texture texture = new Texture(file, index);
                _textures.Add(texture);
                index++;
            }
        }
    }
}