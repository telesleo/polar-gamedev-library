using Microsoft.Xna.Framework;
using Polar.Managers;

namespace Polar
{
    public abstract class Drawer : Component
    {
        protected DrawerManager _drawerManager;

        public float Depth;
        public int Order;
        public int LightLayer;

        public Drawer(float depth, int order, int lightLayer = 0)
        {
            Depth = depth;
            Order = order;
            LightLayer = lightLayer;
        }

        public override void Initialize(Segment segment)
        {
            base.Initialize(segment);
            _drawerManager = segment.DrawerManager;
            _drawerManager.Add(this);
        }

        public override void Unload()
        {
            base.Unload();
            _drawerManager.Remove(this);
        }

        public abstract void Draw();
    }
}
