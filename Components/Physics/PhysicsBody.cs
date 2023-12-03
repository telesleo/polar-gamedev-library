using Microsoft.Xna.Framework;

namespace Polar {
    public class PhysicsBody : Component {
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public float Mass;
        public float Drag;

        public PhysicsBody(float mass = 1f, Vector2 velocity = default, float drag = 0f) {
            Mass = mass;
            Velocity = velocity == default ? new Vector2(0, 0) : velocity;
            Drag = drag;

            _executionOrder = 1000;
        }

        public override void Update(GameTime gameTime) {
            ApplyDrag(gameTime);

            Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameObject.Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void ApplyDrag(GameTime gameTime) {
            Velocity = Vector2.Lerp(Velocity, Vector2.Zero, Drag * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void OnCollide(Collider collider, Vector2 direction) {
            float velocityInDirection = Vector2.Dot(Velocity, direction);
            Vector2 velocityPerpendicular = Velocity - velocityInDirection * direction;
            Vector2 zeroedVelocity = velocityPerpendicular;
            Velocity = zeroedVelocity;
        }
    }
}
