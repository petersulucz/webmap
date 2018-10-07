using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webmap.Data;

namespace WebMap.Api.Models
{
    public class MapData
    {
        public List<PolyLine> Lines { get; }

        public Coordinate MinBound { get; }

        public Coordinate MaxBound { get; }

        public MapData(List<PolyLine> lines, Coordinate minBound, Coordinate maxBound)
        {
            this.Lines = lines;
            this.MinBound = minBound;
            this.MaxBound = maxBound;
        }
    }
}
