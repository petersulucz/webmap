using System;
using System.Collections.Generic;
using System.Linq;
using Webmap.Common;
using Webmap.Common.Primitives;

namespace Webmap.Data
{
    internal class MapWay : IBoundable
    {
        /// <summary>
        /// Gets the ID of the way.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the nodes which make up the way.
        /// </summary>
        public IReadOnlyList<MapNode> Nodes { get; }

        /// <summary>
        /// The tags.
        /// </summary>
        public IDictionary<string, string> Tags { get; }

        /// <summary>
        /// Is a closed set.
        /// </summary>
        public bool IsClosed { get; }

        /// <summary>
        /// Gets the min bound.
        /// </summary>
        public Vector2 MinBound { get; }

        /// <summary>
        /// Gets the maximum bound;
        /// </summary>
        public Vector2 MaxBound { get; }

        public MapWay(long id, IReadOnlyList<MapNode> nodes, IEnumerable<KeyValuePair<string, string>> tags)
        {
            this.Id = id;
            this.Nodes = nodes;
            this.Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var tag in tags)
            {
                if (false == this.Tags.ContainsKey(tag.Key))
                {
                    this.Tags.Add(tag);
                }
            }

            this.IsClosed = this.Nodes.First().Index == this.Nodes.Last().Index;

            this.MinBound = this.Nodes.First().Coordinate;
            this.MaxBound = this.Nodes.First().Coordinate;

            foreach (var node in this.Nodes.Skip(1))
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
