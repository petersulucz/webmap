using System;
using System.Collections.Generic;
using System.Text;
using Webmap.Common;
using Webmap.Common.Primitives;
using Webmap.Data;

namespace Webmap.Backend.MapTypes
{
    internal class WaterMapProvider : MapTypeProvider
    {
        public WaterMapProvider(Vector2 minDimension, Vector2 maxDimension)
            : base(minDimension, maxDimension)
        {

        }

        public override string Name => "Water";

        public override bool IsMatch(MapShape mapWay)
        {
            if(mapWay.Tags.ContainsKey("waterway"))
            {
                return true;
            }

            string type;
            if (mapWay.Tags.TryGetValue("natural", out type) && string.Equals(type, "water", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
