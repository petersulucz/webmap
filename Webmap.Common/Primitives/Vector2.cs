using System;

namespace Webmap.Common.Primitives
{
    public struct Vector2
    {
        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        public double X;

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        public double Y;

        /// <summary>
        /// Creates a new vector with the supplied values.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets the vector magnitude.
        /// </summary>
        public double Magnitude => Math.Sqrt(this.X * this.X + this.Y * this.Y);

        public static Vector2 One => new Vector2(1, 1);

        public static Vector2 MinValue => new Vector2(double.MinValue, double.MinValue);

        public static Vector2 MaxValue => new Vector2(double.MaxValue, double.MaxValue);

        public static Vector2 Zero => new Vector2();

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator /(Vector2 a, double b)
        {
            return new Vector2(a.X / b, a.Y / b);
        }

        public static Vector2 operator *(Vector2 a, double b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public static bool operator >(Vector2 a, Vector2 b)
        {
            return a.X > b.X && a.Y > b.Y;
        }

        public static bool operator <(Vector2 a, Vector2 b)
        {
            return a.X < b.X && a.Y < b.Y;
        }

        public static bool operator >=(Vector2 a, Vector2 b)
        {
            return a.X >= b.X && a.Y >= b.Y;
        }

        public static bool operator <=(Vector2 a, Vector2 b)
        {
            return a.X <= b.X && a.Y <= b.Y;
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }

        public static BoundingType BoundingBox(Vector2 aMin, Vector2 aMax, Vector2 bMin, Vector2 bMax)
        {
            var xBound = Vector2.BoundingLine(aMin.X, aMax.X, bMin.X, bMax.X);
            var yBound = Vector2.BoundingLine(aMin.Y, aMax.Y, bMin.Y, bMax.Y);

            if (xBound == BoundingType.Disjoint || yBound == BoundingType.Disjoint)
            {
                return BoundingType.Disjoint;
            }

            if (xBound == BoundingType.Contains && yBound == BoundingType.Contains)
            {
                return BoundingType.Contains;
            }

            return BoundingType.Intersects;
        }

        private static BoundingType BoundingLine(double x1, double x2, double y1, double y2)
        {
            if ((x1 >= y1 && x2 <= y2) || (y1 >= x1 && y2 <= x2))
            {
                return BoundingType.Contains;
            }

            if (x1 >= y1 && x1 <= y2
                || x2 >= y1 && x2 <= y2)
            {
                return BoundingType.Intersects;
            }

            if (y1 >= x1 && y1 <= x2
                || y2 >= x1 && y2 <= x2)
            {
                return BoundingType.Intersects;
            }

            return BoundingType.Disjoint;
        }
    }
}
