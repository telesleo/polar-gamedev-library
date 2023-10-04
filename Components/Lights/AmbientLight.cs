using Microsoft.Xna.Framework;

namespace Polar
{
    public class AmbientLight : Light
    {
        public AmbientLight(Color color, int order, float intensity = 1) : base(color, order, intensity)
        {

        }
    }
}
