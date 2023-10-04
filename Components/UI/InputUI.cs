using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Polar
{
    public class InputUI : UIElement
    {
        public string Content;

        private KeyboardState _previousKeyboardState;

        public InputUI(string intialContent = "", Color color = default, ElementAlignment alignment = default) : base(color, alignment)
        {
            Content = intialContent;
        }

        public override void Initialize(Segment segment)
        {
            base.Initialize(segment);
            _previousKeyboardState = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (_previousKeyboardState.IsKeyUp(key))
                {
                    if (key == Keys.Escape)
                    {

                    }
                    else if (key == Keys.Back)
                    {
                        if (Content.Length <= 0) continue;
                        Content = Content.Substring(0, Content.Length - 1);
                    }
                    else if (key == Keys.Space)
                    {
                        Content += " ";
                    }
                    else if (key != Keys.Enter)
                    {
                        Content += key.ToString().ToLower();
                    }
                }
            }

            _previousKeyboardState = keyboardState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_uiManager.DefaultFont, Content, _position, Color);
        }
    }
}
