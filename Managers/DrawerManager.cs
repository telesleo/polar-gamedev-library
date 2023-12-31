﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Polar.Managers
{
    public class DrawerManager : Manager<Drawer>
    {
        private Dictionary<int, LightLayerGroup> _lightLayerGroups;
        private VisualizerMeshes _visualizerMeshes;
        private RenderTarget2D _colorRenderTarget;
        private RenderTarget2D _lightRenderTarget;
        private RenderTarget2D _screenRenderTarget;
        private Effect _effect;
        private BoundingFrustum _frustum;

        public DrawerManager()
        {
            _lightLayerGroups = new Dictionary<int, LightLayerGroup>();
        }

        public override void Initialize()
        {
            base.Initialize();
            _effect = PolarSystem.Game.Content.Load<Effect>("Shaders/LightEffect");
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
            _screenRenderTarget = new RenderTarget2D(
                graphicsDevice,
                viewport.Width,
                viewport.Height,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                graphicsDevice.PresentationParameters.DepthStencilFormat,
                graphicsDevice.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.PreserveContents
            );
            _visualizerMeshes = new VisualizerMeshes();
        }

        public override void Unload()
        {
            base.Unload();
            _colorRenderTarget.Dispose();
            _lightRenderTarget.Dispose();
            _screenRenderTarget.Dispose();
            _effect.Dispose();
        }

        public bool IsVisible(VertexPositionColorTexture[] vertices)
        {
            Vector3[] points = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                points[i] = vertices[i].Position;
            }
            BoundingBox boundingBox = BoundingBox.CreateFromPoints(points);
            ContainmentType containmentType = _frustum.Contains(boundingBox);
            return containmentType != ContainmentType.Disjoint;
        }

        public void AddMesh(VertexPositionColorTexture[] vertices, int[] indices, Material material, int lightLayer)
        {
            vertices = vertices.ToArray();
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position *= PolarSystem.UnitSize;
            }
            if (!IsVisible(vertices)) return;
            indices = indices.ToArray();
            if (!_lightLayerGroups.ContainsKey(lightLayer))
            {
                _lightLayerGroups.Add(lightLayer, new LightLayerGroup());
            }
            LightLayerGroup lightLayerGroup = _lightLayerGroups[lightLayer];
            if (!lightLayerGroup.MeshGroups.ContainsKey(material))
            {
                lightLayerGroup.MeshGroups.Add(material, new MeshGroup(material));
            }
            MeshGroup meshGroup = lightLayerGroup.MeshGroups[material];
            meshGroup.AddMesh(vertices, indices);
        }

        public void AddVisualizerMesh(VertexPositionColorTexture[] vertices, int[] indices)
        {
            vertices = vertices.ToArray();
            indices = indices.ToArray();
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position *= PolarSystem.UnitSize;
            }
            _visualizerMeshes.AddMesh(vertices, indices);
        }

        private MeshGroup[] GetMeshGroupsFromLightLayer(LightLayerGroup lightLayerGroup)
        {
            List<MeshGroup> meshGroups = new List<MeshGroup>();
            foreach (MeshGroup meshGroup in lightLayerGroup.MeshGroups.Values.ToArray())
            {
                meshGroups.Add(meshGroup);
            }
            return meshGroups.ToArray();
        }

        private MeshGroup[] GetAllMeshGroups()
        {
            List<MeshGroup> meshGroups = new List<MeshGroup>();
            LightLayerGroup[] lightLayerGroups = _lightLayerGroups.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToArray();
            foreach (LightLayerGroup lightLayerGroup in lightLayerGroups)
            {
                meshGroups.AddRange(GetMeshGroupsFromLightLayer(lightLayerGroup));
            }
            return meshGroups.ToArray();
        }

        private void RenderMeshGroup(GraphicsDevice graphicsDevice, Matrix viewProjection, MeshGroup meshGroup)
        {
            VertexPositionColorTexture[] vertices = meshGroup.Vertices.ToArray();
            int[] indices = meshGroup.Indices.ToArray();
            VertexBuffer vertexBuffer = new VertexBuffer(
                graphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.None
            );
            IndexBuffer indexBuffer = new IndexBuffer(
                graphicsDevice, typeof(short), indices.Length, BufferUsage.None
            );
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;
            meshGroup.Material.Effect.Parameters["WorldViewProjection"]?.SetValue(viewProjection);
            meshGroup.Material.Apply(graphicsDevice);
            meshGroup.Material.Effect.Techniques[0].Passes[0].Apply();
            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3
            );
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        private void RenderVisualizers(GraphicsDevice graphicsDevice)
        {
            if (_visualizerMeshes.Vertices.Count <= 0) return;
            graphicsDevice.SetRenderTarget(null);
            VertexPositionColorTexture[] vertices = _visualizerMeshes.Vertices.ToArray();
            int[] indices = _visualizerMeshes.Indices.ToArray();
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.None);
            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.None);
            PolarSystem.VisualizerMaterial.Apply(graphicsDevice);
            PolarSystem.VisualizerMaterial.Effect.Techniques[0].Passes[0].Apply();
            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        private void RenderWithoutLight(GraphicsDevice graphicsDevice, Matrix viewProjection)
        {
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Color.Transparent);
            MeshGroup[] meshGroups = GetAllMeshGroups();
            foreach (MeshGroup meshGroup in meshGroups)
            {
                RenderMeshGroup(graphicsDevice, viewProjection, meshGroup);
            }
            RenderVisualizers(graphicsDevice);
        }

        private void RenderLight(GraphicsDevice graphicsDevice, ScreenVerticesIndices screenVerticesIndices, LightManager lightManager, int lightLayer)
        {
            graphicsDevice.SetRenderTarget(_lightRenderTarget);
            graphicsDevice.Clear(Color.Transparent);
            graphicsDevice.SetVertexBuffer(screenVerticesIndices.VertexBuffer);
            graphicsDevice.Indices = screenVerticesIndices.IndexBuffer;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            AmbientLight ambientLight = lightManager.ActiveAmbientLight;
            if (ambientLight != null && (ambientLight.LightLayersAffected == null || ambientLight.LightLayersAffected.Contains(lightLayer)))
            {
                graphicsDevice.Clear(ambientLight.Color * ambientLight.Intensity);
            }
            VertexPositionColorTexture[] vertices = screenVerticesIndices.Vertices;
            int[] indices = screenVerticesIndices.Indices;
            foreach (PointLight pointLight in lightManager.PointLights)
            {
                if (pointLight.LightLayersAffected == null || pointLight.LightLayersAffected.Contains(lightLayer))
                {
                    _effect.Parameters["LightPosition"].SetValue(pointLight.GameObject.Position * PolarSystem.UnitSize);
                    _effect.Parameters["LightRange"].SetValue(pointLight.Range * PolarSystem.UnitSize);
                    _effect.Parameters["LightColor"].SetValue(pointLight.Color.ToVector4());
                    _effect.Parameters["LightIntensity"].SetValue(pointLight.Intensity);
                    _effect.Techniques[0].Passes[0].Apply();
                    graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
                }
            }
        }

        private ScreenVerticesIndices GetScreenVerticesIndices(GraphicsDevice graphicsDevice, Camera camera)
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
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4]
            {
                new VertexPositionColorTexture(topLeft, Color.White, new Vector2(0, 1)),
                new VertexPositionColorTexture(topRight, Color.White, new Vector2(1, 1)),
                new VertexPositionColorTexture(bottomRight, Color.White, new Vector2(1, 0)),
                new VertexPositionColorTexture(bottomLeft, Color.White, new Vector2(0, 0))
            };
            int[] indices = new int[6]
            {
                0, 1, 2, 0, 2, 3
            };
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.None);
            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.None);
            return new ScreenVerticesIndices(vertices, indices, vertexBuffer, indexBuffer);
        }

        private void BlendTextureLight(GraphicsDevice graphicsDevice, ScreenVerticesIndices screenVerticesIndices)
        {
            graphicsDevice.SetRenderTarget(_screenRenderTarget);
            graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            _effect.Parameters["LightTexture"]?.SetValue(_lightRenderTarget);
            _effect.Parameters["ColorTexture"]?.SetValue(_colorRenderTarget);
            VertexPositionColorTexture[] vertices = screenVerticesIndices.Vertices;
            int[] indices = screenVerticesIndices.Indices;
            graphicsDevice.SetVertexBuffer(screenVerticesIndices.VertexBuffer);
            graphicsDevice.Indices = screenVerticesIndices.IndexBuffer;
            _effect.Techniques[1].Passes[0].Apply();
            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
        }

        private void RenderScreen(GraphicsDevice graphicsDevice, ScreenVerticesIndices screenVerticesIndices, Matrix viewProjection)
        {
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Color.Transparent);
            _effect.Parameters["WorldViewProjection"]?.SetValue(viewProjection);
            _effect.Parameters["ScreenTexture"]?.SetValue(_screenRenderTarget);
            VertexPositionColorTexture[] vertices = screenVerticesIndices.Vertices;
            int[] indices = screenVerticesIndices.Indices;
            graphicsDevice.SetVertexBuffer(screenVerticesIndices.VertexBuffer);
            graphicsDevice.Indices = screenVerticesIndices.IndexBuffer;
            _effect.Techniques[2].Passes[0].Apply();
            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
            RenderVisualizers(graphicsDevice);
        }

        private void RenderLightLayerGroup(GraphicsDevice graphicsDevice, Matrix viewProjection, ScreenVerticesIndices screenVerticesIndices, LightManager lightManager, LightLayerGroup lightLayerGroup, int lightLayer)
        {
            graphicsDevice.SetRenderTarget(_colorRenderTarget);
            graphicsDevice.Clear(Color.Transparent);
            MeshGroup[] meshGroups = GetMeshGroupsFromLightLayer(lightLayerGroup);
            foreach (MeshGroup meshGroup in meshGroups)
            {
                RenderMeshGroup(graphicsDevice, viewProjection, meshGroup);
            }
            _effect.Parameters["WorldViewProjection"].SetValue(viewProjection);
            RenderLight(graphicsDevice, screenVerticesIndices, lightManager, lightLayer);
            BlendTextureLight(graphicsDevice, screenVerticesIndices);
        }

        private void RenderWithLight(GraphicsDevice graphicsDevice, Camera camera, Matrix viewProjection, LightManager lightManager)
        {
            graphicsDevice.SetRenderTarget(_screenRenderTarget);
            graphicsDevice.Clear(Color.Transparent);
            ScreenVerticesIndices screenVerticesIndices = GetScreenVerticesIndices(graphicsDevice, camera);
            List<int> lightLayers = _lightLayerGroups.Keys.ToList();
            lightLayers.Sort((a, b) => a - b);
            for (int i = 0; i < lightLayers.Count; i++)
            {
                int lightLayer = lightLayers[i];
                RenderLightLayerGroup(graphicsDevice, viewProjection, screenVerticesIndices, lightManager, _lightLayerGroups[lightLayer], lightLayer);
            }
            RenderScreen(graphicsDevice, screenVerticesIndices, viewProjection);
            screenVerticesIndices.VertexBuffer.Dispose();
            screenVerticesIndices.IndexBuffer.Dispose();
        }

        private void DrawDrawers()
        {
            _items.Sort((a, b) => a.Order - b.Order);
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].Draw();
            }
        }

        private Matrix GetViewProjection(Camera camera)
        {
            if (camera == null)
            {
                Viewport viewport = PolarSystem.Game.GraphicsDevice.Viewport;
                return Matrix.CreateOrthographicOffCenter(0, viewport.Width, 0, viewport.Height, 0f, 1f);
            }
            camera.UpdateMatrices();
            return camera.ViewMatrix * camera.ProjectionMatrix;
        }

        public void RenderMeshes(Camera camera, LightManager lightManager)
        {
            Matrix viewProjection = GetViewProjection(camera);
            _frustum = new BoundingFrustum(viewProjection);
            GraphicsDevice graphicsDevice = PolarSystem.Game.GraphicsDevice;
            DrawDrawers();
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            if (PolarSystem.Lighting)
            {
                RenderWithLight(graphicsDevice, camera, viewProjection, lightManager);
            }
            else
            {
                RenderWithoutLight(graphicsDevice, viewProjection);
            }
            _lightLayerGroups.Clear();
            _visualizerMeshes.Clear();
        }

        private struct MeshGroup
        {
            public List<VertexPositionColorTexture> Vertices;
            public List<int> Indices;
            public Material Material;

            public MeshGroup(Material material)
            {
                Vertices =  new List<VertexPositionColorTexture>();
                Indices = new List<int>();
                Material = material;
            }

            public void AddMesh(VertexPositionColorTexture[] vertices, int[] indices)
            {
                int lastVertexIndex = Vertices.Count;
                Vertices.AddRange(vertices);
                foreach (int index in indices)
                {
                    Indices.Add(lastVertexIndex + index);
                }
            }
        }

        private struct LightLayerGroup
        {
            public Dictionary<Material, MeshGroup> MeshGroups;

            public LightLayerGroup()
            {
                MeshGroups = new Dictionary<Material, MeshGroup>();
            }
        }

        private struct ScreenVerticesIndices
        {
            public VertexPositionColorTexture[] Vertices;
            public int[] Indices;
            public VertexBuffer VertexBuffer;
            public IndexBuffer IndexBuffer;

            public ScreenVerticesIndices(VertexPositionColorTexture[] vertices, int[] indices, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
            {
                Vertices = vertices;
                Indices = indices;
                VertexBuffer = vertexBuffer;
                IndexBuffer = indexBuffer;
            }
        }

        private struct VisualizerMeshes
        {
            public List<VertexPositionColorTexture> Vertices;
            public List<int> Indices;

            public VisualizerMeshes()
            {
                Vertices = new List<VertexPositionColorTexture>();
                Indices = new List<int>();
            }

            public void AddMesh(VertexPositionColorTexture[] vertices, int[] indices)
            {
                int lastVertexIndex = Vertices.Count;
                Vertices.AddRange(vertices);
                foreach (int index in indices)
                {
                    Indices.Add(lastVertexIndex + index);
                }
            }

            public void Clear()
            {
                Vertices.Clear();
                Indices.Clear();
            }
        }
    }
}
