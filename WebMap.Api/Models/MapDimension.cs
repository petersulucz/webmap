using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMap.Api.Models
{
    public class MapDimension
    {
        public MapDimension(Coordinate min, Coordinate max)
        {
            this.Max = max;
            this.Min = min;
        }

        public Coordinate Min { get; }

        public Coordinate Max { get; }
    }
}
