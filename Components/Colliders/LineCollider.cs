using Microsoft.Xna.Framework;

namespace Polar
{
    public class LineCollider : Collider {
        public Vector2 PointA;
        public Vector2 PointB;

        public LineCollider(Vector2 pointA, Vector2 pointB, bool @fixed = true) : base(@fixed) {
            PointA = pointA;
            PointB = pointB;
        }

        public override void Update(GameTime gameTime) {
    
        }

        public Vector2[] GetWorldPoints() {
            Vector2[] worldPoints = new Vector2[2];
            worldPoints[0] = GameObject.Position + PointA;
            worldPoints[1] = GameObject.Position + PointB;
            return worldPoints;
        }
    }
}
