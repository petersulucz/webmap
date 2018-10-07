using System;
using System.Collections.Generic;
using System.Text;
using Webmap.Common;
using Webmap.Data;

namespace Webmap.Backend.MapTypes
{
    internal class SchoolMapProvider : MapTypeProvider
    {
        public SchoolMapProvider(Vector2 minDimension, Vector2 maxDimension)
            : base(minDimension, maxDimension)
        {
        }

        public override string Name => "Schools";

        public override bool IsMatch(MapWay mapWay)
        {
            if (mapWay.Tags.TryGetValue("amenity", out var amenityType) && string.Equals("school", amenityType, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
