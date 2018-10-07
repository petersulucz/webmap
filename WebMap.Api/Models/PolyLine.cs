using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webmap.Data;

namespace WebMap.Api.Models
{
    public class PolyLine
    {
        public List<Coordinate> Coordinates { get; }

        public bool IsClosed { get; }

        public PolyLine(MapWay way)
        {
            this.Coordinates = way.Nodes.Select(n => new Coordinate(n.Coordinate)).ToList();
            this.IsClosed = way.IsClosed;
        }
    }
}
