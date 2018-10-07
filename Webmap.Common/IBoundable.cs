using System;
using System.Collections.Generic;
using System.Text;

namespace Webmap.Common
{
    public interface IBoundable
    {
        BoundingType CheckBounding(Vector2 min, Vector2 max);
    }
}
