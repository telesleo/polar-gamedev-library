using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Polar.Managers
{
    public class Manager<T>
    {
        protected List<T> _items;
        public int Order;

        public Manager(int order = 0)
        {
            _items = new List<T>();
        }

        public virtual void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(typeof(T).Name + " item");
            if (_items.Contains(item)) return;
            _items.Add(item);
        }

        public virtual void Remove(T item)
        {
            if (!_items.Contains(item)) return;
            _items.Remove(item);
        }

        public virtual void RemoveAll()
        {
            _items.Clear();
        }

        public virtual void Initialize()
        {

        }

        public virtual void LoadContent()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }

        public virtual void Unload()
        {
            RemoveAll();
        }
    }
}
