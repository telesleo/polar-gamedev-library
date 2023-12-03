using Microsoft.Xna.Framework;

namespace Polar.Managers {
    public class CameraManager : Manager<Camera> {
        public Camera ActiveCamera {
            get {
                if (_items.Count > 0)
                {
                    return _items[_items.Count - 1];
                }
                return null;
            }
        }

        public override void Add(Camera camera) {
            base.Add(camera);
            _items.Sort((a, b) => a.Order - b.Order);
        }

        public override void Update(GameTime gameTime) {
            if (ActiveCamera != null)
            {
                ActiveCamera.UpdateMatrices();
            };
        }
    }
}
