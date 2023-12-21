namespace Polar
{
    public class CircleCollider : Collider {
        public float Radius;

        public CircleCollider(float radius = 16f, bool @fixed = true) : base(@fixed) {
            Radius = radius;
        }
    }
}
