using Microsoft.Xna.Framework;

namespace Polar
{
    public class Component
    {
        public GameObject GameObject;

        internal int _executionOrder;

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
    }
}
