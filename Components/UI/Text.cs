using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Polar
{
    public class Text : UIElement
    {
        public string Content;

        public Text(string content = "", Color color = default, ElementAlignment alignment = default) : base(color, alignment)
        {
            Content = content;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_uiManager.DefaultFont, Content, _position, Color);
        }
    }
}
