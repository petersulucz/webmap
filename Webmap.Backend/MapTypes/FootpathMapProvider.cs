using System;
using System.Collections.Generic;
using System.Text;
using Webmap.Common;
using Webmap.Common.Primitives;
using Webmap.Data;

namespace Webmap.Backend.MapTypes
{
    internal class FootpathMapProvider : MapTypeProvider
    {
        public FootpathMapProvider(Vector2 minDimension, Vector2 maxDimension)
            : base (minDimension, maxDimension)
        {

        }

        public override string Name => "Footpaths";

        public override bool IsMatch(MapShape mapWay)
        {
            if (mapWay.Tags.TryGetValue("highway", out var type))
            {
                return string.Equals(type, "footway", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
