using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polar.Managers;

namespace Polar
{
    public class Component
    {
        public GameObject GameObject;

        public int _executionOrder;

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

        public void DrawVizualizerCircle(DrawerManager drawerManager, Vector2 position, float radius, Color color)
        {
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[PolarSystem.VisualizerCircleVertices.Length];
            int[] indices = PolarSystem.VisualizerCircleIndices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 coord = PolarSystem.VisualizerCircleVertices[i];
                Vector2 vertexPosition = position + coord * radius;
                vertices[i] = new VertexPositionColorTexture(new Vector3(vertexPosition.X, vertexPosition.Y, 0), color, coord);
            }
            drawerManager.AddVisualizerMesh(vertices, indices);
        }
    }
}
