using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Polar {
    public class Input {
        private static Vector2 _axis;
        public static Vector2 Axis => _axis;
        private static bool _jump;
        public static bool Jump => _jump;
        private bool _pressingJump;

        public static bool O { get; private set; }
        public static bool L { get; private set; }

        public Input() {
            _axis = new Vector2();
            _jump = false;
            _pressingJump = false;
        }

        public void Update(GameTime gameTime) {
            _axis.X = 0;
            _axis.Y = 0;
            _jump = false;

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down)) {
                _axis.Y -= 1;
            }
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up)) {
                _axis.Y += 1;
            }
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left)) {
                _axis.X -= 1;
            }
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right)) {
                _axis.X += 1;
            }

            if (keyboardState.IsKeyDown(Keys.O)) {
                O = true;
            } else {
                O = false;
            }

            if (keyboardState.IsKeyDown(Keys.L)) {
                L = true;
            } else {
                L = false;
            }


            if (keyboardState.IsKeyDown(Keys.Space)) {
                if (!_pressingJump) {
                    _jump = true;
                }
                _pressingJump = true;
            } else {
                _pressingJump = false;
            }
        }
    }
}
