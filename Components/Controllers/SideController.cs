using Microsoft.Xna.Framework;

namespace Polar {
    public class SideController : Component {
        private PhysicsBody _physicsBody;

        public float Speed;
        public float JumpForce;
        public float Gravity;

        public SideController(float gravity = 9.8f, float speed = 5f, float jumpForce = 10f) {
            Gravity = gravity;
            Speed = speed;
            JumpForce = jumpForce;
        }

        public override void Initialize(Segment segment) {
            _physicsBody = GameObject.GetComponent<PhysicsBody>();
        }

        public override void Update(GameTime gameTime) {
            Vector2 movement = Input.Axis;

            _physicsBody.Velocity.X += Speed * 50f * (float)gameTime.ElapsedGameTime.TotalSeconds * movement.X;

            if (Input.Jump) {
                _physicsBody.Velocity.Y = JumpForce * 50f;
            }

            _physicsBody.Velocity.Y -= Gravity * 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
