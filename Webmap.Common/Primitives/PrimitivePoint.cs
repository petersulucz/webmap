using System;
using System.Collections.Generic;
using System.Text;

namespace Webmap.Common.Primitives
{
    public class PrimitivePoint : Primitive
    {
        public Vector2 Point { get; }

        public PrimitivePoint(Vector2 point)
            : base (point, point)
        {
            this.Point = point;
        }
    }
}
