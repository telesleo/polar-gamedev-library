using Microsoft.Xna.Framework;

namespace Polar.Managers
{
    public class ColliderManager : Manager<Collider>
    {
        public Color DrawingColor;

        public bool DrawColliders;

        public ColliderManager()
        {
            DrawColliders = false;
            DrawingColor = Color.Yellow;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateColliders();
        }

        private Collider[] GetColliders(Collider colliderA, Collider colliderB)
        {
            if (colliderA is CircleCollider)
            {
                return new Collider[] { colliderA, colliderB };
            }
            if (colliderB is CircleCollider)
            {
                return new Collider[] { colliderB, colliderA};
            }
            return new Collider[] { colliderA, colliderB };
        }

        private void UpdateColliders()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                for (int j = i + 1; j < _items.Count; j++)
                {
                    Collider[] colliders = GetColliders(_items[i], _items[j]);
                    Collider colliderA = colliders[0];
                    Collider colliderB = colliders[1];
                    if (colliderA.GameObject.Awake == false || colliderB.GameObject.Awake == false) continue;
                    if (colliderA.GameObject == colliderB.GameObject) continue;
                    Vector2 vector = Vector2.Zero;
                    if (colliderA is CircleCollider && colliderB is CircleCollider)
                    {
                        vector = Collider.CircleCollision((CircleCollider)colliderA, (CircleCollider)colliderB);
                    }
                    else if (colliderA is CircleCollider && colliderB is LineCollider)
                    {
                        vector = Collider.CircleLineCollision((CircleCollider)colliderA, (LineCollider)colliderB);
                    }
                    else if (colliderA is CircleCollider && colliderB is PolylineCollider)
                    {
                        vector = Collider.CirclePolylineCollision((CircleCollider)colliderA, (PolylineCollider)colliderB);
                    }
                    if (vector != Vector2.Zero)
                    {
                        if (!colliderA.Fixed)
                        {
                            colliderA.GameObject.Position -= vector * (colliderB.Fixed ? 1 : 0.5f);
                            colliderA.GameObject.OnCollide(colliderB, Vector2.Normalize(vector));
                        }
                        if (!colliderB.Fixed)
                        {
                            colliderB.GameObject.Position += vector * (colliderA.Fixed ? 1 : 0.5f);
                            colliderB.GameObject.OnCollide(colliderA, -Vector2.Normalize(vector));
                        }
                    }
                }
            }
        }
    }
}
