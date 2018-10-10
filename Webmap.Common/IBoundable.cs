using System;
using System.Collections.Generic;
using System.Text;
using Webmap.Common.Primitives;

namespace Webmap.Common
{
    public interface IBoundable
    {
        BoundingType CheckBounding(Vector2 min, Vector2 max);
    }
}
