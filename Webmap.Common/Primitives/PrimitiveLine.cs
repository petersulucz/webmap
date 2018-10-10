using System;
using System.Collections.Generic;
using System.Text;

namespace Webmap.Common.Primitives
{
    public class PrimitiveLine : Primitive
    {
        private readonly List<PrimitivePoint> points;

        public PrimitiveLine(IList<PrimitivePoint> points)
            : base(points)
        {
            this.points = new List<PrimitivePoint>(points);
        }

        public IReadOnlyList<PrimitivePoint> Points => this.points;
    }
}
