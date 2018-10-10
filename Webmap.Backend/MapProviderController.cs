using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Webmap.Backend.MapTypes;
using Webmap.Common;
using Webmap.Common.Primitives;
using Webmap.Data;

namespace Webmap.Backend
{
    public class MapProviderController
    {
        private readonly MapDefinition definition;

        private MapProviderController(MapDefinition definition, List<MapTypeProvider> providers)
        {
            this.definition = definition;
            this.Providers = providers.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);
        }

        public Vector2 LowBound => this.definition.MinBound;
        public Vector2 HighBound => this.definition.MaxBound;

        public IDictionary<string, MapTypeProvider> Providers { get; }

        public static MapProviderController Create(MapDefinition mapDefinition)
        {
            var providers = new List<MapTypeProvider>(MapProviderController.CreateProviders(mapDefinition));

            foreach(var way in mapDefinition.ReadWays())
            {
                var provider = providers.FirstOrDefault(p => p.IsMatch(way));
                if (null != provider)
                {
                    provider.Add(way);
                }
            }

            return new MapProviderController(mapDefinition, providers);
        }

        private static IEnumerable<MapTypeProvider> CreateProviders(MapDefinition definition)
        {
            yield return new FootpathMapProvider(definition.MinBound, definition.MaxBound);
            yield return new WaterMapProvider(definition.MinBound, definition.MaxBound);
            yield return new ParkMapProvider(definition.MinBound, definition.MaxBound);
            yield return new SchoolMapProvider(definition.MinBound, definition.MaxBound);
            yield return new HighwaysMapProvider(definition.MinBound, definition.MaxBound);
            yield return new RoadsMapProvider(definition.MinBound, definition.MaxBound);
            yield return new HouseProvider(definition.MinBound, definition.MaxBound);
        }
    }
}
