using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Polar.Managers;
using System.Collections.Generic;
using System.IO;

namespace Polar {
    public sealed class PolarSystem {
        public static int UnitSize = 32;
        public static bool DisplayFPS;
        public static bool DrawColliders = false;
        public static bool Lighting = false;

        public static Game Game { get; private set; }
        public Input Input { get; private set; }
        public SegmentManager SegmentManager { get; private set; }

        private static Dictionary<string, Texture2D> _textures;

        public string ProjectName { get; set; }

        private int _frameCount = 0;
        private float _elapsedTime = 0f;
        private float _updateInterval = 1.0f;
        public int FPS { get; private set; }

        public PolarSystem(Game game) {
            Game = game;

            DisplayFPS = false;

            Input = new Input();
            SegmentManager = new SegmentManager();

            ProjectName = Game.GetType().Namespace;
        }

        public void Initialize() {
            _textures = new Dictionary<string, Texture2D>();
            SegmentManager.Initialize();
        }

        public void LoadContent() {
            SegmentManager.LoadContent();
        }

        public void Update(GameTime gameTime) {
            Input.Update(gameTime);
            SegmentManager.Update(gameTime);
            DisplayCurrentFPS(gameTime);
        }

        public void Draw(GameTime gameTime) {
            SegmentManager.Draw(gameTime);
        }

        public void Unload()
        {
            SegmentManager.ActiveSegment?.Unload();
        }

        public static T LoadPreset<T>(string path)
        {
            string jsonFilePath = Path.Combine(Game.Content.RootDirectory, path);
            string json = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static Texture2D GetTexture(string path)
        {
            if (!_textures.ContainsKey(path))
            {
                Texture2D texture = Game.Content.Load<Texture2D>(path);
                _textures[path] = texture;
                return texture;
            }
            else
            {
                return _textures[path];
            }
        }

        public static void ClearTextures()
        {
            _textures.Clear();
        }

        private void DisplayCurrentFPS(GameTime gameTime) {
            if (!DisplayFPS) {
                return;
            }

            _frameCount += 1;
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_elapsedTime >= _updateInterval) {
                FPS = _frameCount;
                _frameCount = 0;
                _elapsedTime -= _updateInterval;
                Game.Window.Title = $"{ProjectName} - FPS: {FPS}";
            }
        }
    }
}
