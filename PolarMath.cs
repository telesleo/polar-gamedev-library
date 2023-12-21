using Microsoft.Xna.Framework;
using System;

namespace Polar.Math
{
    public sealed class PolarMath
    {
        public static Vector2 PolarToEuclidean(float radius, float angle)
        {
            float x = radius * MathF.Cos(angle);
            float y = radius * MathF.Sin(angle);
            return new Vector2(x, y);
        }

        public static Polar EuclideanToPolar(Vector2 euclidean)
        {
            float radius = euclidean.Length();
            float angle = (float)MathF.Atan2(euclidean.Y, euclidean.X);
            return new Polar(radius, angle);
        }

        public static float Wrap(float value, float range)
        {
            return (value % range + range) % range;
        }

        public static float Map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
        {
            float OldRange = OldMax - OldMin;
            float NewRange = NewMax - NewMin;
            float NewValue = ((OldValue - OldMin) * NewRange / OldRange) + NewMin;
            return NewValue;
        }
    }

    public struct Polar
    {
        public float Radius;
        public float Angle;

        public Polar(float radius, float angle)
        {
            Radius = radius;
            Angle = angle;
        }
    }
}
