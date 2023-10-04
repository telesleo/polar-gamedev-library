using System.Collections.Generic;

namespace Polar.Managers
{
    public class LightManager : Manager<Light>
    {
        public bool Lighting;

        public List<AmbientLight> AmbientLights { get; private set; }
        public List<PointLight> PointLights { get; private set; }

        public AmbientLight ActiveAmbientLight { 
            get {
                if (AmbientLights.Count > 0)
                {
                    return AmbientLights[AmbientLights.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        public LightManager(bool lighting = false)
        {
            AmbientLights = new List<AmbientLight>();
            PointLights = new List<PointLight>();

            Lighting = lighting;
        }

        public void AddLight(Light light)
        {
            if (light is PointLight)
            {
                PointLights.Add((PointLight)light);
                PointLights.Sort((a, b) => a.Order.CompareTo(b.Order));
            }
            else if (light is AmbientLight)
            {
                AmbientLights.Add((AmbientLight)light);
                AmbientLights.Sort((a, b) => a.Order.CompareTo(b.Order));
            }

            Add(light);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();

            AmbientLights.Clear();
            PointLights.Clear();
        }
    }
}
