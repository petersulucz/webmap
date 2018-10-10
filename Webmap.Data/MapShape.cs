using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webmap.Common;
using Webmap.Common.Primitives;

namespace Webmap.Data
{
    public class MapShape : IBoundable
    {
        public Primitive Primitive { get; }

        public IDictionary<string, string> Tags;

        private MapShape(Primitive primitive, IEnumerable<KeyValuePair<string, string>> tags)
        {
            this.Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach(var tag in tags)
            {
                if(false == this.Tags.ContainsKey(tag.Key))
                {
                    this.Tags.Add(tag);
                }
            }

            this.Primitive = primitive;
        }

        internal static MapShape Create(MapWay way, IEnumerable<KeyValuePair<string, string>> tags)
        {
            if (way.IsClosed)
            {
                var len = way.Nodes.Count();
                var outerEdge = new PrimitiveLine(way.Nodes.Take(len - 1).Select(n => new PrimitivePoint(n.Coordinate)).ToList());
                var primitive = new PrimitivePolygon(outerEdge, Enumerable.Empty<PrimitivePolygon>());
                return new MapShape(primitive, tags);
            }
            else
            {
                var primitive = new PrimitiveLine(way.Nodes.Select(n => new PrimitivePoint(n.Coordinate)).ToList());
                return new MapShape(primitive, tags);
            }
        }

        internal static MapShape Create(MapWay outer, IEnumerable<MapWay> inners, IEnumerable<KeyValuePair<string, string>> tags)
        {
            var len = outer.Nodes.Count();
            var outerEdge = new PrimitiveLine(outer.Nodes.Take(len - 1).Select(n => new PrimitivePoint(n.Coordinate)).ToList());
            var innerEdges = inners.Select(inner => new PrimitivePolygon(new PrimitiveLine(inner.Nodes.Take(len - 1).Select(n => new PrimitivePoint(n.Coordinate)).ToList())));

            var primitive = new PrimitivePolygon(outerEdge, innerEdges);

            return new MapShape(primitive, tags);
        }

        public BoundingType CheckBounding(Vector2 min, Vector2 max)
        {
            return this.Primitive.CheckBounding(min, max);
        }
    }
}
