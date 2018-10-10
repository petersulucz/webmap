using System;
using System.Collections.Generic;
using System.Text;
using Webmap.Common;
using Webmap.Common.Primitives;
using Webmap.Data;

namespace Webmap.Backend.MapTypes
{
    internal class HighwaysMapProvider : MapTypeProvider
    {
        public HighwaysMapProvider(Vector2 minDimension, Vector2 maxDimension)
            : base(minDimension, maxDimension)
        {
        }

        public override string Name => "Highways";

        public override bool IsMatch(MapShape mapWay)
        {
            if (mapWay.Tags.TryGetValue("highway", out var heighwayType) && heighwayType.StartsWith("motorway", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
