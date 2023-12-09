using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Polar.Managers
{
    public sealed class GameObjectManager : Manager<GameObject>
    {
        public Segment Segment { get; private set; }

        public override void Remove(GameObject item)
        {
            item.Unload();
            base.Remove(item);
        }

        public void AddAndInitializeGameObject(GameObject gameObject)
        {
            Add(gameObject);
            gameObject.Initialize(Segment);
        }

        public void InitializeGameObjects(Segment segment)
        {
            Segment = segment;
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].Initialize(Segment);
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Enabled)
                {
                    _items[i].Update(gameTime);
                }
            }
        }

        public void DrawVisualizer(DrawerManager drawerManager)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Enabled)
                {
                    _items[i].DrawVisualizer(drawerManager);
                }
            }
        }

        public override void Unload()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].Unload();
            }
        }

        public GameObject[] GetObjectsByTag(string tag)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Tag == tag)
                {
                    gameObjects.Add(_items[i]);
                }
            }
            return gameObjects.ToArray();
        }

        public GameObject GetObjectByName(string name)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Name == name)
                {
                    return _items[i];
                }
            }
            return null;
        }
    }
}
