using Microsoft.Xna.Framework.Graphics;

namespace Polar.Managers
{
    public class UIManager : Manager<UIElement>
    {
        private SpriteBatch _spriteBatch;

        public SpriteFont DefaultFont { get; private set; }

        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(PolarSystem.Game.GraphicsDevice);
        }

        public override void LoadContent()
        {
            DefaultFont = PolarSystem.Game.Content.Load<SpriteFont>("Fonts/DefaultFont");
        }

        public void DrawUIElements()
        {
            _spriteBatch.Begin();
            foreach (var item in _items)
            {
                if (item.GameObject.Enabled)
                {
                    item.Draw(_spriteBatch);
                }
            }
            _spriteBatch.End();
        }
    }
}
