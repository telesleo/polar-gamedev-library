using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Polar {
    public class Camera : Component {
        public Matrix WorldMatrix { get; private set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        public float FieldOfView;
        public float AspectRatio { get; private set; }
        public float NearPlane;
        public float FarPlane;
        public float ZOffset;

        public int Order;

        public Camera(int order = 0)
        {
            Order = order;
            FieldOfView = MathHelper.PiOver2;
            FarPlane = 2048f;
            NearPlane = 0.1f;

        }

        public override void Initialize(Segment segment)
        {
            Viewport viewport = PolarSystem.Game.GraphicsDevice.Viewport;
            AspectRatio = viewport.AspectRatio;
            ZOffset = (float)viewport.Height / 2 / MathF.Tan(FieldOfView / 2) / PolarSystem.UnitSize;
            segment.CameraManager.Add(this);
        }

        public Matrix GetWorldMatrix()
        {
            return Matrix.CreateRotationZ(MathHelper.ToRadians(-GameObject.Rotation)) * Matrix.CreateTranslation(new Vector3(GameObject.Position.X, GameObject.Position.Y, 0) * PolarSystem.UnitSize);
        }

        public Matrix GetViewMatrix() {
            Vector3 position = new Vector3(GameObject.Position.X, GameObject.Position.Y, ZOffset) * PolarSystem.UnitSize;
            Vector3 target = position + new Vector3(0, 0, -1);
            Vector2 up = GameObject.Up;
            return Matrix.CreateLookAt(position, target, new Vector3(up.X, up.Y, 0));
        }

        public Matrix GetProjectionMatrix() {
            return Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
        }

        public void UpdateMatrices() {
            WorldMatrix = GetWorldMatrix();
            ViewMatrix = GetViewMatrix();
            ProjectionMatrix = GetProjectionMatrix();
        }
    }
}
