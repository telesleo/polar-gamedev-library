using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Polar
{
    public struct Material
    {
        public Effect Effect;
        public Dictionary<string, object> Parameters;

        public Material(Effect effect)
        {
            Effect = effect;
            Parameters = new Dictionary<string, object>();
        }

        public void ApplyParameters()
        {
            foreach (var parameter in Parameters)
            {
                if (Effect.Parameters[parameter.Key] == null) continue;

                if (parameter.Value is bool boolValue)
                {
                    Effect.Parameters[parameter.Key].SetValue(boolValue);
                }
                else if (parameter.Value is int intValue)
                {
                    Effect.Parameters[parameter.Key].SetValue(intValue);
                }
                else if (parameter.Value is float floatValue)
                {
                    Effect.Parameters[parameter.Key].SetValue(floatValue);
                }
                else if (parameter.Value is Vector2 vector2Value)
                {
                    Effect.Parameters[parameter.Key].SetValue(vector2Value);
                }
                else if (parameter.Value is Vector3 vector3Value)
                {
                    Effect.Parameters[parameter.Key].SetValue(vector3Value);
                }
                else if (parameter.Value is Vector4 vector4Value)
                {
                    Effect.Parameters[parameter.Key].SetValue(vector4Value);
                }
                else if (parameter.Value is Texture2D texture)
                {
                    Effect.Parameters[parameter.Key].SetValue(texture);
                }
                else if (parameter.Value is Matrix matrix)
                {
                    Effect.Parameters[parameter.Key].SetValue(matrix);
                }
            }
        }
    }
}
