using Microsoft.Xna.Framework;

namespace Polar {
    public class SimpleController : Component {
        public float Speed;

        public SimpleController(float speed = 5f) {
            Speed = speed;
        }

        public override void Update(GameTime gameTime) {
            Vector2 movement = Input.Axis;

            if (movement.Length() > 1) {
                movement = Vector2.Normalize(movement);
            }

            GameObject.LocalPosition += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds * movement;
        }
    }
}
