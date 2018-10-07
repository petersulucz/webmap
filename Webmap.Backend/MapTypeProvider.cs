using System;
using System.Collections.Generic;
using Webmap.Common;
using Webmap.Data;

namespace Webmap.Backend
{
    public abstract class MapTypeProvider
    {
        private readonly QuadTree<MapWay> ways;

        public MapTypeProvider(Vector2 minDimension, Vector2 maxDimension)
        {
            this.ways = new QuadTree<MapWay>(minDimension - Vector2.One, maxDimension + Vector2.One, 6);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets if this is a match.
        /// </summary>
        /// <param name="mapWay">The mapway.</param>
        /// <returns>If it is a match.</returns>
        public abstract bool IsMatch(MapWay mapWay);

        /// <summary>
        /// Adds the way to the tree.
        /// </summary>
        /// <param name="mapWay">The map way.</param>
        public void Add(MapWay mapWay)
        {
            this.ways.AddItem(mapWay);
        }

        /// <summary>
        /// Gets the ways within the dimension.
        /// </summary>
        /// <param name="minDimension">The min dimension.</param>
        /// <param name="maxDimension">The max dimension.</param>
        /// <returns>All of the ways.</returns>
        public IEnumerable<MapWay> GetWays(Vector2 minDimension, Vector2 maxDimension)
        {
            return this.ways.Range(minDimension, maxDimension);
        }
    }
}
