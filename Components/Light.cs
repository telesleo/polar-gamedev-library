using Microsoft.Xna.Framework;

namespace Polar
{
    public class Light : Component
    {
        public Color Color;
        public float Intensity;
        public int Order;
        public int[] LightLayersAffected;

        public Light(Color color, int order, float intensity = 1, int[] lightLayersAffected = null)
        {
            Color = color;
            Order = order;
            Intensity = intensity;
            LightLayersAffected = lightLayersAffected;
        }

        public override void Initialize(Segment segment)
        {
            segment.LightManager.AddLight(this);
        }
    }
}
