using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Polar.Managers;

namespace Polar
{
    public class GameObject
    {
        public Segment Segment { get; private set; }

        public string Name;
        public string Tag;

        public bool Enabled;
        public bool _awake;
        public bool Awake
        {
            get
            {
                if (ParentObject != null)
                {
                    return ParentObject.Awake;
                }
                return _awake;
            }
            set
            {
                _awake = value;
            }
        }

        public List<Component> Components { get; private set; }
        public GameObject ParentObject;
        public List<GameObject> ChildrenObjects { get; private set; }

        public Vector2 LocalPosition;
        public Vector2 LocalScale;
        public float LocalRotation;
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

        public GameObject(string name,Vector2 position, Vector2 scale, float rotation, string tag = null, bool enabled = true, bool awake = true)
        {
            Name = name;
            Enabled = enabled;
            _awake = awake;

            Components = new List<Component>();
            ChildrenObjects = new List<GameObject>();

            LocalPosition = position;
            LocalScale = scale;
            LocalRotation = rotation;
            Tag = tag;
        }

        public void Initialize(Segment segment)
        {
            Segment = segment;
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Initialize(segment);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Update(gameTime);
            }
        }

        public void Unload()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Unload();
            }
        }

        public void OnCollide(Collider collider, Vector2 direction)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].OnCollide(collider, direction);
            }
        }

        public void ApplyMotion(Vector2 motion)
        {
            Position += motion;
        }

        public void DrawVisualizer(DrawerManager drawerManager)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Component component = Components[i];
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
            for (int i = 0; i < Components.Count; i++)
            {
                Component component = Components[i];
                if (component is T) {
                    return (T)component;
                }
            }
            return default;
        }

        public T[] GetComponents<T>() where T : Component
        {
            List<T> components = new List<T>();
            for (int i = 0; i < Components.Count; i++)
            {
                Component component = Components[i];
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
            for (int i = 0; i < ChildrenObjects.Count; i++)
            {
                component = ChildrenObjects[i].GetChildComponent<T>();
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
            for (int i = 0; i < ChildrenObjects.Count; i++)
            {
                components.AddRange(ChildrenObjects[i].GetChildComponents<T>());
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
            return Segment.GameObjectManager.GetObjectsByTag(tag);
        }

        public void SetParent(GameObject parent)
        {
            Vector2 position = Position;
            ParentObject?.RemoveChildObject(this);
            if (parent != null)
            {
                parent.AddChildObject(this);
            }
            Position = position;
        }
    }
}
