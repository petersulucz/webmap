using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webmap.Common.Primitives;
using Webmap.Data;
using WebMap.Api.Models;

namespace WebMap.Api.Services
{
    public static class PolyshapeBuilder
    {
        public static PolyShape Create(MapShape shape)
        {
            if (shape.Primitive is PrimitiveLine line)
            {
                return PolyshapeBuilder.Create(line);
            }
            else if (shape.Primitive is PrimitivePolygon polygon)
            {
                return PolyshapeBuilder.Create(polygon);
            }
            else
            {
                throw new NotImplementedException("Something is missing yo.");
            }
        }

        private static PolyShape Create(PrimitivePolygon polygon)
        {
            var type = PolyShapeType.Polygon.ToString();
            var coordinates = new List<Coordinate>(polygon.OuterShape.Points.Select(n => new Coordinate(n.Point)));

            if (polygon.InnerShapes.Any())
            {
                return new ComplexPolyShape(coordinates, polygon.InnerShapes.Select(PolyshapeBuilder.Create).ToList(), type);
            }

            return new PolyShape(coordinates, type);
        }

        private static PolyShape Create(PrimitiveLine line)
        {
            var type = PolyShapeType.Line.ToString();
            var coordinates = new List<Coordinate>(line.Points.Select(n => new Coordinate(n.Point)));
            return new PolyShape(coordinates, type);
        }
    }
}
