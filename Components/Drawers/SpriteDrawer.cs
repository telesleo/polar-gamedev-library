using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Polar
{
    public class SpriteDrawer : Drawer
    {
        public string SpritePath;
        public Color Color;

        private Material _material;

        private float _radiansRotation;

        public SpriteDrawer(string spritePath, Color color = default, float depth = 0f, int order = 0) : base(depth, order)
        {
            SpritePath = spritePath;
            Color = color == default ? Color.White : color;
        }

        public override void Initialize(Segment segment)
        {
            base.Initialize(segment);
            Texture2D sprite = PolarSystem.GetTexture(SpritePath);
            _material = new Material(PolarSystem.Game.Content.Load<Effect>("Shaders/Effect"));
            _material.Parameters.Add("Texture", sprite);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _radiansRotation = MathHelper.ToRadians(-GameObject.Rotation);
        }

        public override void DrawerDraw()
        {
            Vector2 position = GameObject.Position;
            Texture2D texture = (Texture2D)_material.Parameters["Texture"];
            float width = texture.Width * GameObject.Scale.X / PolarSystem.UnitSize;
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
                new VertexPositionColorTexture(positionA, Color, new Vector2(0, 1)),
                new VertexPositionColorTexture(positionB, Color, new Vector2(0, 0)),
                new VertexPositionColorTexture(positionC, Color, new Vector2(1, 0)),
                new VertexPositionColorTexture(positionD, Color, new Vector2(1, 1))
            };
            int[] indices = new int[6]
            {
                0, 1, 2, 0, 2, 3
            };
            _drawerManager.AddShape(_material, vertices, indices, Order);
        }
    }
}
