using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polar.Managers;

namespace Polar
{
    public abstract class UIElement : Component
    {
        protected UIManager _uiManager;
        public Color Color;

        public ElementAlignment Alignment;
        protected Vector2 _position;

        public abstract void Draw(SpriteBatch spriteBatch);

        public UIElement(Color color = default, ElementAlignment alignment = default)
        {
            if (color == default)
            {
                color = Color.Black;
            }
            Color = color;
            Alignment = alignment;
        }

        public override void Initialize(Segment segment)
        {
            base.Initialize(segment);
            _uiManager = segment.UIManager;
            _uiManager.Add(this);
            _position = new Vector2(0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            Viewport viewport = PolarSystem.Game.GraphicsDevice.Viewport;
            switch(Alignment)
            {
                case ElementAlignment.TopLeft:
                    _position.X = GameObject.Position.X;
                    _position.Y = GameObject.Position.Y;
                    break;
                case ElementAlignment.TopRight:
                    _position.X = viewport.Width - GameObject.Position.X;
                    _position.Y = GameObject.Position.Y;
                    break;
                case ElementAlignment.BottomLeft:
                    _position.X = GameObject.Position.X;
                    _position.Y = viewport.Height - GameObject.Position.Y;
                    break;
                case ElementAlignment.BottomRight:
                    _position.X = viewport.Width - GameObject.Position.X;
                    _position.Y = viewport.Height - GameObject.Position.Y;
                    break;
                default:
                    break;
            }
        }
    }

    public enum ElementAlignment
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
