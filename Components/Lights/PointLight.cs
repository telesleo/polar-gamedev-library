using Microsoft.Xna.Framework;

namespace Polar
{
    public class PointLight : Light
    {
        public float Range;

        public PointLight(Color color, int order, float range, float intensity = 1, int[] lightLayerAffected = null) : base(color, order, intensity, lightLayerAffected)
        {
            Range = range;
        }
    }
}
