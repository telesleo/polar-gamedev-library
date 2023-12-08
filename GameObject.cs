using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Polar.Managers;

namespace Polar
{
    public class GameObject
    {
        public GameObjectManager GameObjectManager { get; private set; }

        public string Name;
        public string Tag;

        public bool Enabled;

        public Vector2 LocalPosition;
        public Vector2 LocalScale;
        public float LocalRotation;

        public List<Component> Components { get; private set; }
        public GameObject ParentObject;
        public List<GameObject> ChildrenObjects { get; private set; }

        public Vector2 Position
        {
            get
            {
                if (ParentObject != null)
                {
                    Matrix scaleMatrix = Matrix.CreateScale(ParentObject.Scale.X, ParentObject.Scale.Y, 1f);
                    float rotation = -MathHelper.ToRadians(ParentObject.Rotation);
                    Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);
                    Matrix translationMatrix = Matrix.CreateTranslation(ParentObject.Position.X, ParentObject.Position.Y, 0f);
                    Matrix worldMatrix = scaleMatrix * rotationMatrix * translationMatrix;
                    return Vector2.Transform(LocalPosition, worldMatrix);
                }
                return LocalPosition;
            }
            set
            {
                if (ParentObject != null)
                {
                    Matrix scaleMatrix = Matrix.CreateScale(ParentObject.Scale.X, ParentObject.Scale.Y, 1f);
                    float rotation = -MathHelper.ToRadians(ParentObject.Rotation);
                    Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);
                    Matrix translationMatrix = Matrix.CreateTranslation(ParentObject.Position.X, ParentObject.Position.Y, 0f);
                    Matrix worldMatrix = scaleMatrix * rotationMatrix * translationMatrix;
                    LocalPosition = Vector2.Transform(value, Matrix.Invert(worldMatrix));
                } 
                else
                {
                    LocalPosition = value;
                }
            }
        }
        public Vector2 Scale
        {
            get
            {
                if (ParentObject != null)
                {
                    return ParentObject.Scale * LocalScale;
                }
                return LocalScale;
            }
            set
            {
                if (ParentObject != null)
                {
                    LocalScale = value / ParentObject.Scale;
                }
                else
                {
                    LocalScale = value;
                }
            }
        }
        public float Rotation
        {
            get
            {
                if (ParentObject != null)
                {
                    return ParentObject.Rotation + LocalRotation;
                }
                return LocalRotation;
            }
            set
            {
                if (ParentObject != null)
                {
                    LocalRotation = value - ParentObject.Rotation;
                }
                else
                {
                    LocalRotation = value;
                }
            }
        }

        public Vector2 Up
        {
            get
            {
                float rotationInRadians = MathHelper.ToRadians(Rotation);
                return new Vector2((float)MathF.Sin(rotationInRadians), (float)MathF.Cos(rotationInRadians));
            }
            set
            {
                Rotation = MathHelper.ToDegrees(MathF.Atan2(value.X, value.Y));
            }
        }

        public GameObject(string name,Vector2 position, Vector2 scale, float rotation, string tag = null, bool enabled = true)
        {
            Name = name;
            Enabled = enabled;

            Components = new List<Component>();
            ChildrenObjects = new List<GameObject>();

            LocalPosition = position;
            LocalScale = scale;
            LocalRotation = rotation;
            Tag = tag;
        }

        public void Initialize(Segment segment)
        {
            GameObjectManager = segment.GameObjectManager;
            foreach (Component component in Components)
            {
                component.Initialize(segment);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Component component in Components)
            {
                component.Update(gameTime);
            }
        }

        public void Unload()
        {
            foreach (Component component in Components)
            {
                component.Unload();
            }
        }

        public void OnCollide(Collider collider, Vector2 direction)
        {
            foreach (Component component in Components)
            {
                component.OnCollide(collider, direction);
            }
        }

        public void DrawVisualizer(DrawerManager drawerManager)
        {
            foreach (Component component in Components)
            {
                if (component.Visualizer)
                {
                    component.DrawVisualizer(drawerManager);
                }
            }
        }

        public void AddComponent(Component component)
        {
            component.GameObject = this;
            Components.Add(component);
            Components.Sort((a, b) => a._executionOrder - b._executionOrder);
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in Components)
            {
                if (component is T) {
                    return (T)component;
                }
            }
            return default;
        }

        public T[] GetComponents<T>() where T : Component
        {
            List<T> components = new List<T>();
            foreach (Component component in Components)
            {
                if (component is T)
                {
                    components.Add((T)component);
                }
            }
            return components.ToArray();
        }

        public T GetChildComponent<T>() where T : Component
        {
            T component = GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            foreach (var child in ChildrenObjects)
            {
                component = child.GetChildComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }

        public T[] GetChildComponents<T>() where T : Component
        {
            List<T> components = new List<T>();
            components.AddRange(GetComponents<T>());
            foreach (var child in ChildrenObjects)
            {
                components.AddRange(child.GetChildComponents<T>());
            }
            return components.ToArray();
        }

        public void AddChildObject(GameObject gameObject)
        {
            ChildrenObjects.Add(gameObject);
            gameObject.ParentObject = this;
        }

        public void RemoveChildObject(GameObject gameObject)
        {
            ChildrenObjects.Remove(gameObject);
            gameObject.ParentObject = null;
        }

        public GameObject[] GetObjectsByTag(string tag)
        {
            return GameObjectManager.GetObjectsByTag(tag);
        }

        public void SetParent(GameObject parent)
        {
            ParentObject?.RemoveChildObject(this);
            if (parent != null)
            {
                parent.AddChildObject(this);
            }

        }
    }
}
