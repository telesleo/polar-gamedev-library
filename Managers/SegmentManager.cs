﻿using Microsoft.Xna.Framework;

namespace Polar.Managers
{
    public class SegmentManager : Manager<Segment>
    {
        private int _currentSegmentIndex;
        public Segment ActiveSegment
        { 
            get
            {
                if (_items.Count <= 0) return null;
                return _items[_currentSegmentIndex];
            }
        }

        public SegmentManager()
        {
            _currentSegmentIndex = 0;
        }

        public void ReloadSegment()
        {
            PolarSystem.ClearTextures();
            Segment segment = ActiveSegment;
            segment.Unload();
            segment.Initialize();
            segment.LoadContent();
        }

        public void ChangeSegment(int segmentIndex)
        {
            if (segmentIndex < _items.Count && segmentIndex != _currentSegmentIndex)
            {
                ActiveSegment.Unload();
                _currentSegmentIndex = segmentIndex;
                ReloadSegment();
            }
        }

        public override void Initialize()
        {
            ActiveSegment?.Initialize();
        }

        public override void LoadContent()
        {
            ActiveSegment?.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ActiveSegment?.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ActiveSegment?.Draw(gameTime);
        }
    }
}
