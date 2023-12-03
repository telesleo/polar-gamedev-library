using Microsoft.Xna.Framework;
using Polar.Managers;
using System.Collections.Generic;

namespace Polar
{
    public class Collider : Component {
        protected ColliderManager _colliderManager;

        public bool Fixed;

        public Collider(bool @fixed) {
            Fixed = @fixed;
        }

        public override void Initialize(Segment segment)
        {
            _colliderManager = segment.ColliderManager;
            _colliderManager.Add(this);
        }

        public static Vector2 CircleCollision(CircleCollider circleColliderA, CircleCollider circleColliderB) {
            Vector2 positionA = circleColliderA.GameObject.Position;
            Vector2 positionB = circleColliderB.GameObject.Position;
            float radiusA = circleColliderA.Radius/* * (PolarSystem.UnitSize)*/;
            float radiusB = circleColliderB.Radius/* * (PolarSystem.UnitSize)*/;

            float distance = Vector2.Distance(positionA, positionB);

            if (distance < radiusA + radiusB) {
                Vector2 direction = Vector2.Normalize(positionB - positionA);
                float amount = radiusA + radiusB - distance;
                return direction * amount;
            }
            return Vector2.Zero;
        }



        public static Vector2 CircleLineCollision(CircleCollider circleCollider, LineCollider lineCollider) {
            Vector2 circlePosition = circleCollider.GameObject.Position;
            float circleRadius = circleCollider.Radius;

            Vector2[] worldPoints = lineCollider.GetWorldPoints();
            Vector2 pointA = worldPoints[0];
            Vector2 pointB = worldPoints[1];

            Vector2 closestPoint = GetLineClosestPoint(pointA, pointB, circlePosition);
            float distance = Vector2.Distance(closestPoint, circlePosition);
            if (distance < circleRadius) {
                Vector2 direction = Vector2.Normalize(closestPoint - circlePosition);
                float amount = circleRadius - distance;
                return direction * amount;
            }
            return Vector2.Zero;
        }

        public static Vector2 CirclePolylineCollision(CircleCollider circleCollider, PolylineCollider polylineCollider) {
            Vector2 circlePosition = circleCollider.GameObject.Position;
            float circleRadius = circleCollider.Radius;

            Vector2[][] worldPolylines = polylineCollider.GetWorldPolylines();
            List<Vector2> collisions = new List<Vector2>();

            foreach (Vector2[] polyline in worldPolylines) {
                for (int i = 0; i < polyline.Length - 1; i++) {
                    Vector2 pointA = polyline[i];
                    Vector2 pointB = polyline[i + 1];
                    Vector2 closestPoint = GetLineClosestPoint(pointA, pointB, circlePosition);
                    float distance = Vector2.Distance(closestPoint, circlePosition);
                    if (distance < circleRadius) {
                        Vector2 direction = Vector2.Normalize(closestPoint - circlePosition);
                        float amount = circleRadius - distance;
                        collisions.Add(direction * amount);
                    }
                }
            }

            Vector2 finalCollision = Vector2.Zero;

            foreach (Vector2 collision in collisions) {
                finalCollision += collision;
            }

            return finalCollision;
        }

        public static Vector2 GetLineClosestPoint(Vector2 pointA, Vector2 pointB, Vector2 comparisonPoint) {
            float length = Vector2.Distance(pointA, pointB);
            Vector2 vector1 = comparisonPoint - pointA;
            Vector2 vector2 = pointB - pointA;
            float t = Vector2.Dot(vector1, vector2) / (length * length);
            float tClamped = MathHelper.Clamp(t, 0, 1);
            return Vector2.Lerp(pointA, pointB, tClamped);
        }

        public virtual void DrawCollider() {

        }

        public struct Collision {
            public Vector2 Direction;
            public float Amount;

            public Collision(Vector2 direction, float amount) {
                Direction = direction;
                Amount = amount;
            }
        }
    }
}
