using Microsoft.Xna.Framework;
using Polar.Managers;

namespace Polar
{
    public class Component
    {
        public GameObject GameObject;

        internal int _executionOrder;

        public bool Visualizer = false;

        public virtual void Initialize(Segment segment)
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Unload()
        {

        }

        public virtual void OnCollide(Collider collider, Vector2 direction)
        {

        }

        public virtual void DrawVisualizer(DrawerManager drawerManager)
        {

        }
    }
}
