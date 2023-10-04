using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Polar
{
    public class MeshDrawer : Drawer
    {
        public SubMesh[] SubMeshes;

        public MeshDrawer(SubMesh[] subMeshes, float depth = 0, int order = 0) : base(depth, order)
        {
            SubMeshes = subMeshes;
        }

        public override void DrawerDraw()
        {
            Vector2 scale = GameObject.Scale;
            Matrix scaleMatrix = Matrix.CreateScale(scale.X, scale.Y, 0);
            Matrix rotationMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(-GameObject.Rotation));
            Vector2 position = GameObject.Position * PolarSystem.UnitSize;
            Matrix translationMatrix = Matrix.CreateTranslation(new Vector3(position.X, position.Y, Depth * PolarSystem.UnitSize));
            foreach (SubMesh subMesh in SubMeshes)
            {
                VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = new VertexPositionColorTexture(subMesh.Vertices[i].Position, subMesh.Vertices[i].Color, subMesh.Vertices[i].TextureCoordinate);
                    vertices[i].Position = Vector3.Transform(vertices[i].Position * PolarSystem.UnitSize, scaleMatrix * rotationMatrix * translationMatrix);
                }
                _drawerManager.AddShape(PolarSystem.GetTexture(subMesh.TexturePath), vertices, subMesh.Indices, Order);
            }
        }
    }

    public class SubMesh
    {
        public string TexturePath;
        public VertexPositionColorTexture[] Vertices;
        public int[] Indices;

        public SubMesh(string texturePath, VertexPositionColorTexture[] vertices, int[] indices)
        {
            TexturePath = texturePath;
            Vertices = vertices;
            Indices = indices;
        }
    }
}
