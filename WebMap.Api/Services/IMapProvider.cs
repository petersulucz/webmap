using System.Collections.Generic;
using Webmap.Backend;
using Webmap.Common;
using Webmap.Common.Primitives;
using Webmap.Data;
using WebMap.Api.Models;

namespace WebMap.Api.Services
{
    public interface IMapProvider
    {
        /// <summary>
        /// Gets the map data for a provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="lowBound">The low bound.</param>
        /// <param name="highBound">The high bound.</param>
        /// <returns>The map data.</returns>
        MapData GetMapData(string provider, Vector2 lowBound, Vector2 highBound);

        /// <summary>
        /// Gets the dimension of the available map.
        /// </summary>
        /// <returns>The dimension of the map.</returns>
        MapDimension GetDimension();

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <returns>The providers.</returns>
        IEnumerable<string> GetProviders();

        /// <summary>
        /// Sets the map.
        /// </summary>
        /// <param name="map">The map.</param>
        void SetMap(MapProviderController provider);

        /// <summary>
        /// Returns true if this object is ready.
        /// </summary>
        /// <returns>True if the object is ready.</returns>
        bool IsReady();
    }
}
