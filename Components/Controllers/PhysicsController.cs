using Microsoft.Xna.Framework;

namespace Polar {
    public class PhysicsController : Component {
        private PhysicsBody _physicsBody;

        public float Gravity;
        public float Speed;

        public PhysicsController(float speed = 5f) {
            Speed = speed;
        }

        public override void Initialize(Segment segment) {
            _physicsBody = GameObject.GetComponent<PhysicsBody>();
        }

        public override void Update(GameTime gameTime) {
            Vector2 movement = Input.Axis;

            if (movement.Length() > 1) {
                movement = Vector2.Normalize(movement);
            }

            _physicsBody.Velocity += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds * movement;
        }
    }
}
