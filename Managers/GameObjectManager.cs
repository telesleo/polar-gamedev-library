using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Polar.Managers
{
    public sealed class GameObjectManager : Manager<GameObject>
    {
        private Segment _segment;

        public override void Remove(GameObject item)
        {
            item.Unload();
            base.Remove(item);
        }

        public void AddAndInitializeGameObject(GameObject gameObject)
        {
            Add(gameObject);
            gameObject.Initialize(_segment);
        }

        public void InitializeGameObjects(Segment segment)
        {
            _segment = segment;
            foreach (GameObject gameObject in _items)
            {
                gameObject.Initialize(_segment);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameObject gameObject in _items)
            {
                if (gameObject.Enabled)
                {
                    gameObject.Update(gameTime);
                }
            }
        }

        public void DrawVisualizer(DrawerManager drawerManager)
        {
            foreach (GameObject gameObject in _items)
            {
                if (gameObject.Enabled)
                {
                    gameObject.DrawVisualizer(drawerManager);
                }
            }
        }

        public override void Unload()
        {
            foreach (GameObject gameObject in _items)
            {
                gameObject.Unload();
            }
        }

        public GameObject[] GetObjectsByTag(string tag)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            foreach (GameObject gameObject in _items) 
            {
                if (gameObject.Tag == tag)
                {
                    gameObjects.Add(gameObject);
                }
            }
            return gameObjects.ToArray();
        }
    }
}
