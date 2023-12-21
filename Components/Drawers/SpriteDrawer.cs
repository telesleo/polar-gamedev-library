using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polar.Managers;

namespace Polar
{
    public class SpriteDrawer : Drawer
    {
        public string SpritePath;
        public Color Color;
        private bool _flipX;
        private int _spriteCount;
        public int SpriteCount 
        { 
            get 
            { 
                return _spriteCount; 
            } 
            set 
            { 
                _spriteCount = value;
                UpdateUV();
            } 
        }
        private int _spriteIndex;
        public int SpriteIndex
        {
            get
            {
                return _spriteIndex;
            }
            set
            {
                _spriteIndex = value;
                UpdateUV();
            }
        }
        public bool FlipX { 
            get 
            {
                return _flipX; 
            }
            set
            {
                _flipX = value;
                UpdateUV();
            }
        }
        private bool _flipY;
        public bool FlipY
        {
            get
            {
                return _flipY;
            }
            set
            {
                _flipY = value;
                UpdateUV();
            }
        }
        private Material _material;
        private float _radiansRotation;
        private Vector2[] _uv;

        public SpriteDrawer(string spritePath, Color color = default, int spriteCount = 1, int spriteIndex = 0, float depth = 0f, int order = 0, int lightLayer = 0) : base(depth, order, lightLayer)
        {
            SpritePath = spritePath;
            Color = color == default ? Color.White : color;
            SpriteCount = spriteCount;
            SpriteIndex = spriteIndex;
        }

        public override void Initialize(Segment segment)
        {
            base.Initialize(segment);
            _material = new Material(PolarSystem.Game.Content.Load<Effect>("Shaders/Effect"));
            Texture2D sprite = PolarSystem.GetTexture(SpritePath);
            _material.Parameters.Add("Texture", sprite);
            UpdateUV();
        }

        public void UpdateUV()
        {
            float x1 = 0;
            float x2 = 1;
            float y1 = 0;
            float y2 = 1;
            if (SpriteCount > 1)
            {
                float uvPiece = 1f / SpriteCount;
                x1 = SpriteIndex * uvPiece;
                x2 = SpriteIndex * uvPiece + uvPiece;
            }
            _uv = new Vector2[4]
            {
                new Vector2(FlipX ? x2 : x1, FlipY ? y1 : y2),
                new Vector2(FlipX ? x2 : x1, FlipY ? y2 : y1),
                new Vector2(FlipX ? x1 : x2, FlipY ? y2 : y1),
                new Vector2(FlipX ? x1 : x2, FlipY ? y1 : y2)
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _radiansRotation = MathHelper.ToRadians(-GameObject.Rotation);
        }

        public override void Draw()
        {
            if (GameObject.Awake)
            {
                DrawSprite();
            }
        }

        private void DrawSprite()
        {
            Vector2 position = GameObject.Position;
            Texture2D texture = (Texture2D)_material.Parameters["Texture"];
            int spriteCount = (SpriteCount <= 1) ? 1 : SpriteCount;
            float width = texture.Width / spriteCount * GameObject.Scale.X / PolarSystem.UnitSize;
            float height = texture.Height * GameObject.Scale.Y / PolarSystem.UnitSize;
            float offsetX = width / 2;
            float offsetY = height / 2;
            Matrix rotationMatrix = Matrix.CreateRotationZ(_radiansRotation);
            Matrix translationMatrix = Matrix.CreateTranslation(position.X, position.Y, Depth);
            Vector3 positionA = Vector3.Transform(new Vector3(-offsetX, -offsetY, 0), rotationMatrix * translationMatrix);
            Vector3 positionB = Vector3.Transform(new Vector3(-offsetX, offsetY, 0), rotationMatrix * translationMatrix);
            Vector3 positionC = Vector3.Transform(new Vector3(offsetX, offsetY, 0), rotationMatrix * translationMatrix);
            Vector3 positionD = Vector3.Transform(new Vector3(offsetX, -offsetY, 0), rotationMatrix * translationMatrix);
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4]
            {
                new VertexPositionColorTexture(positionA, Color, _uv[0]),
                new VertexPositionColorTexture(positionB, Color, _uv[1]),
                new VertexPositionColorTexture(positionC, Color, _uv[2]),
                new VertexPositionColorTexture(positionD, Color, _uv[3])
            };
            int[] indices = new int[6]
            {
                0, 1, 2, 0, 2, 3
            };
            _drawerManager.AddMesh(vertices, indices, _material, LightLayer);
        }
    }
}
