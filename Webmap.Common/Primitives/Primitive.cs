using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webmap.Common.Primitives
{
    public abstract class Primitive : IBoundable
    {
        private readonly Vector2 lowerBound;
        private readonly Vector2 upperBound;

        protected Primitive(IEnumerable<PrimitivePoint> children)
        {
            this.CalculateBounds(children, out this.lowerBound, out this.upperBound);
        }

        protected Primitive(Vector2 lowerBound, Vector2 upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        public Vector2 LowerBound => this.lowerBound;

        public Vector2 UpperBound => this.upperBound;

        public BoundingType CheckBounding(Vector2 min, Vector2 max)
        {
            return Vector2.BoundingBox(min, max, this.lowerBound, this.upperBound);
        }

        private void CalculateBounds(IEnumerable<PrimitivePoint> points, out Vector2 lowBound, out Vector2 upperBound)
        {
            Vector2 min = Vector2.MaxValue;
            Vector2 max = Vector2.MinValue;
            foreach(var point in points)
            {
                min = Vector2.Min(point.Point, min);
                max = Vector2.Max(point.Point, max);
            }

            lowBound = min;
            upperBound = max;
        }
    }
}
