using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webmap.Common.Primitives;
using Webmap.Data;

namespace WebMap.Api.Models
{
    public class PolyShape
    {
        public List<Coordinate> Coordinates { get; }

        public string Type { get; }

        public PolyShape(List<Coordinate> coordinates, string type)
        {
            this.Coordinates = coordinates;
            this.Type = type;
        }
    }
}
