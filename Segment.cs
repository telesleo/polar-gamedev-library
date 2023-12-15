using Microsoft.Xna.Framework;
using Polar.Managers;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Polar
{
    public class Segment
    {
        public string InitialGameObjects { get; private set; }

        public GameObjectManager GameObjectManager { get; private set; }
        public ColliderManager ColliderManager { get; private set; }
        public CameraManager CameraManager { get; private set; }
        public LightManager LightManager { get; private set; }
        public DrawerManager DrawerManager { get; private set; }
        public UIManager UIManager { get; private set; }

        private JsonSerializerSettings _jsonSettings;

        public Segment()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects
            };
        }

        public void AddInitialGameObjects(params GameObject[] gameObjects)
        {
            List<GameObject> rootGameObjects = new List<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.ParentObject == null)
                {
                    rootGameObjects.Add(gameObject);
                }
            }
            InitialGameObjects = JsonConvert.SerializeObject(rootGameObjects, _jsonSettings);
        }

        private void AddGameObject(GameObject gameObject)
        {
            GameObjectManager.Add(gameObject);
            foreach (Component component in gameObject.Components)
            {
                component.GameObject = gameObject;
            }
            foreach (GameObject childGameObject in gameObject.ChildrenObjects)
            {
                childGameObject.ParentObject = gameObject;
                AddGameObject(childGameObject);
            }
        }

        public void Initialize()
        {
            CameraManager = new CameraManager();
            GameObjectManager = new GameObjectManager();
            LightManager = new LightManager();
            DrawerManager = new DrawerManager();
            ColliderManager = new ColliderManager();
            UIManager = new UIManager();

            GameObject[] gameObjects = JsonConvert.DeserializeObject<GameObject[]>(InitialGameObjects, _jsonSettings);
            foreach (GameObject gameObject in gameObjects)
            {
                AddGameObject(gameObject);
            }

            GameObjectManager.InitializeGameObjects(this);
            ColliderManager.Initialize();
            DrawerManager.Initialize();
            UIManager.Initialize();
        }

        public void LoadContent() {
            DrawerManager.LoadContent();
            UIManager.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            GameObjectManager.Update(gameTime);
            ColliderManager.Update(gameTime);
            CameraManager.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            DrawerManager.RenderMeshes(CameraManager.ActiveCamera, LightManager);
            GameObjectManager.DrawVisualizer(DrawerManager);
            UIManager.DrawUIElements();
        }

        public void Unload()
        {
            CameraManager?.Unload();
            GameObjectManager?.Unload();
            LightManager?.Unload();
            DrawerManager?.Unload();
            ColliderManager?.Unload();
            UIManager.Unload();
        }
    }
}
