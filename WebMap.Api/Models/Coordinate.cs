using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webmap.Common;
using Webmap.Common.Primitives;

namespace WebMap.Api.Models
{
    public class Coordinate : IBoundable
    {
        public double Latitude { get; }

        public double Longitude { get; }


        public Coordinate(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public Coordinate(Vector2 coordinate)
        {
            this.Latitude = coordinate.Y;
            this.Longitude = coordinate.X;
        }

        public BoundingType CheckBounding(Vector2 min, Vector2 max)
        {
            if(this.Latitude > min.Y && this.Latitude <= max.Y
                && this.Longitude > min.X && this.Longitude <= max.X)
            {
                return BoundingType.Contains;
            }

            return BoundingType.Disjoint;
        }
    }
}
