using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polar.Math;

namespace Polar.Managers
{
    public class DrawerManager : Manager<Drawer>
    {
        private Dictionary<string, ShapeGroup> _shapeGroups;

        private Effect _effect;

        private RenderTarget2D _lightRenderTarget;
        private RenderTarget2D _colorRenderTarget;

        private Texture2D _visualizerTexture;

        public override void Initialize()
        {
            base.Initialize();
            _shapeGroups = new Dictionary<string, ShapeGroup>();

            GraphicsDevice graphicsDevice = PolarSystem.Game.GraphicsDevice;
            Viewport viewport = graphicsDevice.Viewport;
            _colorRenderTarget = new RenderTarget2D(
                graphicsDevice,
                viewport.Width,
                viewport.Height
            );
            _lightRenderTarget = new RenderTarget2D(
                graphicsDevice,
                viewport.Width / 8,
                viewport.Height / 8
            );
            _visualizerTexture = new Texture2D(PolarSystem.Game.GraphicsDevice, 1, 1);
            _visualizerTexture.SetData(new Color[] { Color.White });
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _effect = PolarSystem.Game.Content.Load<Effect>("Shaders/Effect");
        }

        public override void Unload()
        {
            base.Unload();
            _colorRenderTarget.Dispose();
            _lightRenderTarget.Dispose();
            _visualizerTexture.Dispose();
        }

        public override void Add(Drawer item)
        {
            base.Add(item);
            _items.Sort((a, b) => {
                int orderPriority = a.Order.CompareTo(b.Order);
                if (orderPriority == 0)
                {
                    return a.Depth.CompareTo(b.Depth);
                }
                return orderPriority;
            });
        }

        public void AddShape(Texture2D texture, VertexPositionColorTexture[] vertices, int[] indices, int order)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position *= PolarSystem.UnitSize;
            }
            string key = new StringBuilder().Append(texture.Name).Append(order).ToString();
            if (!_shapeGroups.ContainsKey(key))
            {
                _shapeGroups.Add(key, new ShapeGroup(texture));
            }
            ShapeGroup shapeGroup = _shapeGroups[key];
            int indexOffset = shapeGroup.Vertices.Count;
            shapeGroup.Vertices.AddRange(vertices);
            for (int i = 0; i < indices.Length; i++)
            {
                shapeGroup.Indices.Add(indexOffset + indices[i]);
            }
        }

        private void RenderShape(GraphicsDevice graphicsDevice, ShapeGroup shapeGroup)
        {
            VertexPositionColorTexture[] vertices = shapeGroup.Vertices.ToArray();
            int[] indices = shapeGroup.Indices.ToArray();
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.None);
            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.None);
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;
            _effect.Parameters["Texture"].SetValue(shapeGroup.Texture);
            _effect.Techniques[0].Passes[0].Apply();
            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        private void RenderLight(GraphicsDevice graphicsDevice, Camera camera, LightManager lightManager)
        {
            Viewport viewport = graphicsDevice.Viewport;
            float width = viewport.Width;
            float height = viewport.Height;
            Matrix worldMatrix = Matrix.Identity;
            if (camera != null)
            {
                float cameraZOffset = camera.ZOffset;
                float cameraFieldOfView = camera.FieldOfView;
                float halfScreenHeight = cameraZOffset * MathF.Tan(cameraFieldOfView / 2) * PolarSystem.UnitSize;

                height = halfScreenHeight * 2;
                width = height * viewport.AspectRatio;

                worldMatrix = Matrix.CreateTranslation(-width / 2, -height / 2, 0) * camera.WorldMatrix;
            }
            Vector3 topLeft = new Vector3(0, 0, 0);
            Vector3 topRight = new Vector3(width, 0, 0);
            Vector3 bottomRight = new Vector3(width, height, 0);
            Vector3 bottomLeft = new Vector3(0, height, 0);
            topLeft = Vector3.Transform(topLeft, worldMatrix);
            topRight = Vector3.Transform(topRight, worldMatrix);
            bottomRight = Vector3.Transform(bottomRight, worldMatrix);
            bottomLeft = Vector3.Transform(bottomLeft, worldMatrix);
            VertexPositionTexture[] vertices = new VertexPositionTexture[4]
            {
                    new VertexPositionTexture(topLeft, new Vector2(0, 1)),
                    new VertexPositionTexture(topRight, new Vector2(1, 1)),
                    new VertexPositionTexture(bottomRight, new Vector2(1, 0)),
                    new VertexPositionTexture(bottomLeft, new Vector2(0, 0))
            };
            int[] indices = new int[6]
            {
                    0, 1, 2, 0, 2, 3
            };
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.None);
            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.None);
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            graphicsDevice.SetRenderTarget(_lightRenderTarget);
            AmbientLight ambientLight = lightManager.ActiveAmbientLight;
            if (ambientLight != null)
            {
                graphicsDevice.Clear(ambientLight.Color * ambientLight.Intensity);
            }
            foreach (PointLight pointLight in lightManager.PointLights)
            {
                _effect.Parameters["LightPosition"].SetValue(pointLight.GameObject.Position * PolarSystem.UnitSize);
                _effect.Parameters["LightRange"].SetValue(pointLight.Range * PolarSystem.UnitSize);
                _effect.Parameters["LightColor"].SetValue(pointLight.Color.ToVector4());
                _effect.Parameters["LightIntensity"].SetValue(pointLight.Intensity);
                _effect.Techniques[1].Passes[0].Apply();
                graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
            }

            graphicsDevice.SetRenderTarget(null);
            _effect.Parameters["LightTexture"].SetValue(_lightRenderTarget);
            _effect.Parameters["ColorTexture"].SetValue(_colorRenderTarget);
            _effect.Techniques[2].Passes[0].Apply();
            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        public void DrawAll(Camera camera, LightManager lightManager)
        {
            foreach (Drawer drawer in _items)
            {
                drawer.DrawerDraw();
            }

            GraphicsDevice graphicsDevice = PolarSystem.Game.GraphicsDevice;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            _effect.Parameters["WorldViewProjection"].SetValue(GetWorldViewProjectionMatrix(camera));

            if (lightManager != null && PolarSystem.Lighting)
            {
                graphicsDevice.SetRenderTarget(_colorRenderTarget);
            }
            else
            {
                graphicsDevice.SetRenderTarget(null);
            }
            foreach (ShapeGroup shapeGroup in _shapeGroups.Values) {
                RenderShape(graphicsDevice, shapeGroup);
            }
            if (lightManager != null && PolarSystem.Lighting)
            {
                RenderLight(graphicsDevice, camera, lightManager);
            }
            _shapeGroups.Clear();
        }

        public Matrix GetWorldViewProjectionMatrix(Camera camera)
        {
            if (camera != null)
            {
                return camera.ViewMatrix * camera.ProjectionMatrix;
            }
            else
            {
                Viewport viewport = PolarSystem.Game.GraphicsDevice.Viewport;
                return Matrix.CreateOrthographicOffCenter(0, viewport.Width, 0, viewport.Height, 0f, 1f);
            }
        }

        public struct ShapeGroup
        {
            public Texture2D Texture;
            public List<VertexPositionColorTexture> Vertices;
            public List<int> Indices;

            public ShapeGroup(Texture2D texture)
            {
                Texture = texture;
                Vertices = new List<VertexPositionColorTexture>();
                Indices = new List<int>();
            }
        }
    }
}
