using Microsoft.Xna.Framework;
using Polar.Math;
using System.Collections.Generic;

namespace Polar
{
    public class SpriteAnimator : Component
    {
        private AnimatorState _animationState;
        private SpriteDrawer _spriteDrawer;

        public Dictionary<string, Animation> Animations;

        private float _elapsedTime;
        private int _currentFrameIndex;

        public SpriteAnimator(Dictionary<string, Animation> animations)
        {
            Animations = animations;
        }

        public override void Initialize(Segment segment)
        {
            base.Initialize(segment);
            _animationState = GameObject.GetComponent<AnimatorState>();
            _spriteDrawer = GameObject.GetComponent<SpriteDrawer>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Animation animation = Animations[_animationState.State];

            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _currentFrameIndex = (int)PolarMath.Wrap(_elapsedTime / animation.Rate, (float)animation.Sprites.Length);
            if (_spriteDrawer.SpriteIndex != animation.Sprites[_currentFrameIndex])
            {
                _spriteDrawer.SpriteIndex = animation.Sprites[_currentFrameIndex];
            }
        }

        public struct Animation
        {
            public int[] Sprites;
            public float Rate;

            public Animation(int[] sprites, float rate)
            {
                Sprites = sprites;
                Rate = rate;
            }
        }
    }
}
