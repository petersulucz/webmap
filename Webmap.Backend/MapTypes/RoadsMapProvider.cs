using System;
using System.Collections.Generic;
using System.Text;
using Webmap.Common;
using Webmap.Data;

namespace Webmap.Backend.MapTypes
{
    internal class RoadsMapProvider : MapTypeProvider
    {
        public RoadsMapProvider(Vector2 minDimension, Vector2 maxDimension)
            : base(minDimension, maxDimension)
        {
        }

        public override string Name => "Roads";

        public override bool IsMatch(MapWay mapWay)
        {
            if (mapWay.Tags.TryGetValue("highway", out var heighwayType) && !heighwayType.StartsWith("motorway", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
