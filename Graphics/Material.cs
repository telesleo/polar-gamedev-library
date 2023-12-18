using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Polar
{
    public struct Material
    {
        public Effect Effect;
        public Dictionary<string, object> Parameters;
        public SamplerState[] SamplerStates;

        public Material(Effect effect, SamplerState[] samplerStates = null)
        {
            Effect = effect;
            Parameters = new Dictionary<string, object>();
            SamplerStates = samplerStates;
        }

        private void ApplySamplerStates(GraphicsDevice graphicsDevice)
        {
            if (SamplerStates != null)
            {
                for (int i = 0; i < SamplerStates.Length; i++)
                {
                    graphicsDevice.SamplerStates[i] = SamplerStates[i];
                }
            }
            else
            {
                graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            }
        }

        private void ApplyParameters()
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
                else if (parameter.Value is Texture2D textureValue)
                {
                    Effect.Parameters[parameter.Key].SetValue(textureValue);
                }
                else if (parameter.Value is Matrix matrixValue)
                {
                    Effect.Parameters[parameter.Key].SetValue(matrixValue);
                }
                else if (parameter.Value is Color colorValue)
                {
                    Effect.Parameters[parameter.Key].SetValue(colorValue.ToVector4());
                }
            }
        }

        public void Apply(GraphicsDevice graphicsDevice)
        {
            ApplySamplerStates(graphicsDevice);
            ApplyParameters();
        }
    }
}
