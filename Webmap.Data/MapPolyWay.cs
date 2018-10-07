using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webmap.Common;

namespace Webmap.Data
{
    public class MapPolyWay
    {
        /// <summary>
        /// Gets the ID of the way.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the nodes which make up the way.
        /// </summary>
        public IReadOnlyList<MapWay> Outer { get; }

        /// <summary>
        /// The inner map ways.
        /// </summary>
        public IReadOnlyList<MapWay> Inner { get; }

        /// <summary>
        /// The tags.
        /// </summary>
        public IDictionary<string, string> Tags { get; }

        /// <summary>
        /// Is a closed set.
        /// </summary>
        public bool IsClosed => true;

        /// <summary>
        /// Gets the min bound.
        /// </summary>
        public Vector2 MinBound { get; }

        /// <summary>
        /// Gets the maximum bound;
        /// </summary>
        public Vector2 MaxBound { get; }

        public MapPolyWay(long id, IReadOnlyList<MapWay> outer, IReadOnlyList<MapWay> inner, IEnumerable<KeyValuePair<string, string>> tags)
        {
            this.Id = id;
            this.Outer = outer;
            this.Inner = inner;
            this.Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var tag in tags)
            {
                if (false == this.Tags.ContainsKey(tag.Key))
                {
                    this.Tags.Add(tag);
                }
            }

            //this.IsClosed = this.Outer.First() == this.Nodes.Last();

            this.MinBound = this.Outer.First().Nodes.First().Coordinate;
            this.MaxBound = this.Outer.First().Nodes.First().Coordinate;

            foreach (var node in this.Outer.SelectMany(o => o.Nodes).Skip(1))
            {
                this.MinBound = Vector2.Min(this.MinBound, node.Coordinate);
                this.MaxBound = Vector2.Max(this.MaxBound, node.Coordinate);
            }
        }

        public BoundingType CheckBounding(Vector2 min, Vector2 max)
        {
            return Vector2.BoundingBox(min, max, this.MinBound, this.MaxBound);
        }
    }
}
