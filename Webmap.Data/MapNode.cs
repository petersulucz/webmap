using Webmap.Common;

namespace Webmap.Data
{
    public class MapNode
    {
        public long Index { get; }

        public Vector2 Coordinate { get; }

        public MapNode(long index, Vector2 coordinate)
        {
            this.Index = index;
            this.Coordinate = coordinate;
        }
    }
}
