using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Polar {
    public class Input {
        public static Vector2 Axis;
        public static Vector2 RightAxis;
        public static Vector2 LeftAxis;
        public static bool Jump;
        private bool _pressingJump;

        public static bool O { get; private set; }
        public static bool L { get; private set; }
        public static bool LeftShift { get; private set; }
        public static bool LeftControl { get; private set; }

        public Input() {
            Axis = new Vector2();
            LeftAxis = new Vector2();
            RightAxis = new Vector2();
            Jump = false;
            _pressingJump = false;
        }

        public void Update(GameTime gameTime) {
            Axis.X = 0;
            Axis.Y = 0;
            LeftAxis.X = 0;
            LeftAxis.Y = 0;
            RightAxis.X = 0;
            RightAxis.Y = 0;
            Jump = false;

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down)) {
                Axis.Y -= 1;
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    LeftAxis.Y -= 1;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    RightAxis.Y -= 1;
                }
            }
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up)) {
                Axis.Y += 1;
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    LeftAxis.Y += 1;
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    RightAxis.Y += 1;
                }
            }
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left)) {
                Axis.X -= 1;
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    LeftAxis.X -= 1;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    RightAxis.X -= 1;
                }
            }
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right)) {
                Axis.X += 1;
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    LeftAxis.X += 1;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    RightAxis.X += 1;
                }
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
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                LeftShift = true;
            }
            else
            {
                LeftShift = false;
            }
            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                LeftControl = true;
            }
            else
            {
                LeftControl = false;
            }

            if (keyboardState.IsKeyDown(Keys.Space)) {
                if (!_pressingJump) {
                    Jump = true;
                }
                _pressingJump = true;
            } else {
                _pressingJump = false;
            }
        }
    }
}
