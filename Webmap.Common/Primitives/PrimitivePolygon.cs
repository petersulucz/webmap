using System.Collections.Generic;
using System.Linq;

namespace Webmap.Common.Primitives
{
    public class PrimitivePolygon : Primitive
    {
        private readonly List<PrimitivePolygon> innerShapes;
        
        public PrimitivePolygon(PrimitiveLine outer)
            : this(outer, Enumerable.Empty<PrimitivePolygon>())
        { }

        public PrimitivePolygon(PrimitiveLine outer, IEnumerable<PrimitivePolygon> inner)
            : base (outer.LowerBound, outer.UpperBound)
        {
            this.OuterShape = outer;
            this.innerShapes = new List<PrimitivePolygon>(inner);
        }

        public PrimitiveLine OuterShape { get; }

        public IReadOnlyList<PrimitivePolygon> InnerShapes => this.innerShapes;
    }
}
