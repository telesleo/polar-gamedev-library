using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Polar {
    public class SpriteDrawer : Drawer {
        public string SpritePath;
        public Color Color;

        public Texture2D Sprite { get; private set; }

        private float _radiansRotation;

        public SpriteDrawer(string spritePath, Color color = default, float depth = 0f, int order = 0) : base(depth, order) {
            SpritePath = spritePath;
            Color = color == default ? Color.White : color;
        }

        public override void Initialize(Segment segment)
        {
            base.Initialize(segment);
            Sprite = PolarSystem.GetTexture(SpritePath);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _radiansRotation = MathHelper.ToRadians(-GameObject.Rotation);
        }

        public override void DrawerDraw()
        {
            Vector2 position = GameObject.Position * PolarSystem.UnitSize;
            float width = Sprite.Width * GameObject.Scale.X;
            float height = Sprite.Height * GameObject.Scale.Y;
            float offsetX = width / 2;
            float offsetY = height / 2;
            Matrix rotationMatrix = Matrix.CreateRotationZ(_radiansRotation);
            Matrix translationMatrix = Matrix.CreateTranslation(position.X, position.Y, Depth * PolarSystem.UnitSize);
            Vector3 positionA = Vector3.Transform(new Vector3(-offsetX, -offsetY, 0), rotationMatrix * translationMatrix);
            Vector3 positionB = Vector3.Transform(new Vector3(-offsetX, offsetY, 0), rotationMatrix * translationMatrix);
            Vector3 positionC = Vector3.Transform(new Vector3(offsetX, offsetY, 0), rotationMatrix * translationMatrix);
            Vector3 positionD = Vector3.Transform(new Vector3(offsetX, -offsetY, 0), rotationMatrix * translationMatrix);
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4]
            {
                new VertexPositionColorTexture(positionA, Color, new Vector2(0, 1)),
                new VertexPositionColorTexture(positionB, Color, new Vector2(0, 0)),
                new VertexPositionColorTexture(positionC, Color, new Vector2(1, 0)),
                new VertexPositionColorTexture(positionD, Color, new Vector2(1, 1))
            };
            int[] indices = new int[6]
            {
                0, 1, 2, 0, 2, 3
            };
            _drawerManager.AddShape(Sprite, vertices, indices, Order);
        }
    }
}
