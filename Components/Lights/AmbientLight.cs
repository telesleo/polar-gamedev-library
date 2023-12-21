using Microsoft.Xna.Framework;

namespace Polar
{
    public class AmbientLight : Light
    {
        public AmbientLight(Color color, int order, float intensity = 1, int[] lightLayersAffected = null) : base(color, order, intensity, lightLayersAffected)
        {

        }
    }
}
