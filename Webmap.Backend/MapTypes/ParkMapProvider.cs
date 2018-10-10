using System;
using System.Collections.Generic;
using System.Text;
using Webmap.Common;
using Webmap.Common.Primitives;
using Webmap.Data;

namespace Webmap.Backend.MapTypes
{
    internal class ParkMapProvider : MapTypeProvider
    {
        public ParkMapProvider(Vector2 minDimension, Vector2 maxDimension)
            : base(minDimension, maxDimension)
        {
        }

        public override string Name => "Parks";

        public override bool IsMatch(MapShape mapWay)
        {
            if (mapWay.Tags.TryGetValue("leisure", out var leisureType) && string.Equals("park", leisureType, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
