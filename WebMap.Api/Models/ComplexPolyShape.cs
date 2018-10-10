using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webmap.Data;

namespace WebMap.Api.Models
{
    public class ComplexPolyShape : PolyShape
    {
        public IList<PolyShape> InnerShapes { get; }

        public ComplexPolyShape(List<Coordinate> coordinates, List<PolyShape> innerShapes, string type)
            : base (coordinates, type)
        {
            this.InnerShapes = innerShapes;
        }
    }
}
