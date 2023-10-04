using Polar.Managers;

namespace Polar
{
    public abstract class Drawer : Component
    {
        protected DrawerManager _drawerManager;

        public float Depth;
        public int Order;

        public Drawer(float depth, int order)
        {
            Depth = depth;
            Order = order;
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

        public abstract void DrawerDraw();
    }
}
