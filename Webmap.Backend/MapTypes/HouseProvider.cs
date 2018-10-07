using System;
using System.Collections.Generic;
using System.Text;
using Webmap.Common;
using Webmap.Data;

namespace Webmap.Backend.MapTypes
{
    internal class HouseProvider : MapTypeProvider
    {
        public HouseProvider(Vector2 minDimension, Vector2 maxDimension)
            : base(minDimension, maxDimension)
        {
        }

        public override string Name => "Houses";

        public override bool IsMatch(MapWay mapWay)
        {
            if (mapWay.Tags.TryGetValue("building", out var buildingType) 
                && (string.Equals(buildingType, "house", StringComparison.OrdinalIgnoreCase)
                || string.Equals(buildingType, "residential", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }
    }
}
