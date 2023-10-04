using Microsoft.Xna.Framework;
using Polar.Managers;
using System;

namespace Polar
{
    public class CircleCollider : Collider {
        public float Radius;

        public CircleCollider(float radius = 16f, bool @fixed = true) : base(@fixed) {
            Radius = radius;
        }

        public override void DrawCollider() {
            //float radius = Radius;

            //int segments = 28;
            //float angle = MathF.PI * 2 / segments;
            //Vector2 initialPoint = GameObject.Position + PolarMath.PolarToEuclidean(radius, 0);
            //Vector2 prevPoint = initialPoint;
            //for (int i = 1; i < segments; i++) {
            //    Vector2 point = GameObject.Position + PolarMath.PolarToEuclidean(radius, i * angle);
            //    drawer.DrawLine(prevPoint, point, _colliderManager.DrawingColor);
            //    prevPoint = point;
            //}
            //drawer.DrawLine(prevPoint, initialPoint, _colliderManager.DrawingColor);
        }
    }
}
