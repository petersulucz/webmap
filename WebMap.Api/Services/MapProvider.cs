using System.Collections.Generic;
using Webmap.Backend;
using Webmap.Common;
using Webmap.Common.Primitives;
using Webmap.Data;
using WebMap.Api.Models;

namespace WebMap.Api.Services
{
    public class MapProvider : IMapProvider
    {
        /// <summary>
        /// The quad tree.
        /// </summary>
        private MapProviderController provider;

        public MapDimension GetDimension()
        {
            return new MapDimension(new Coordinate(this.provider.LowBound), new Coordinate(this.provider.HighBound));
        }

        /// <summary>
        /// Gets the map data.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="lowBound">The low bound.</param>
        /// <param name="highBound">The high bound.</param>
        /// <returns>The map data.</returns>
        public MapData GetMapData(string type, Vector2 lowBound, Vector2 highBound)
        {
            var mapref = this.provider;
            if (mapref == null || false == mapref.Providers.TryGetValue(type, out var mapProv))
            {
                return new MapData(new List<PolyShape>(), new Coordinate(lowBound), new Coordinate(highBound));
            }

            var lines = new List<PolyShape>();
            foreach (var line in mapProv.GetWays(lowBound, highBound))
            {
                lines.Add(PolyshapeBuilder.Create(line));
            }

            return new MapData(lines, new Coordinate(Vector2.Max(mapref.LowBound, lowBound)), new Coordinate(Vector2.Min(mapref.HighBound, highBound)));
        }

        public IEnumerable<string> GetProviders()
        {
            return this.provider.Providers.Keys;
        }

        public bool IsReady()
        {
            return this.provider != null;
        }

        public void SetMap(MapProviderController provider)
        {
            this.provider = provider;
        }
    }
}
