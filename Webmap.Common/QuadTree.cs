using System;
using System.Collections.Generic;
using Webmap.Common.Primitives;

namespace Webmap.Common
{
    public class QuadTree<T> where T : IBoundable
    {
        private readonly QuadTreeNode<T> root;
        private readonly int maxHeight;

        public QuadTree(Vector2 lowBound, Vector2 highBound, int maxHeight = 30)
        {
            this.root = new QuadTreeNode<T>(this, lowBound, highBound, 0);
            this.maxHeight = maxHeight;
            this.LowBound = lowBound;
            this.HightBound = highBound;
        }

        public Vector2 LowBound { get; }

        public Vector2 HightBound { get; }

        /// <summary>
        /// Iterate over a range of the tree.
        /// </summary>
        /// <param name="lowBound">The low bound.</param>
        /// <param name="highBound">The high bound.</param>
        /// <returns>The matching elements.</returns>
        public IEnumerable<T> Range(Vector2 lowBound, Vector2 highBound)
        {
            return this.root.IterateRange(lowBound, highBound);
        }

        /// <summary>
        /// Adds an item to the tree.
        /// </summary>
        /// <param name="item">The item in the tree.</param>
        public void AddItem(T item)
        {
            this.root.PushItem(item);
        }

        private class QuadTreeNode<T> where T : IBoundable
        {
            private readonly QuadTree<T> parent;
            private readonly int height;
            private readonly Vector2 bottomLeft;
            private readonly Vector2 topRight;
            private readonly Vector2 center;
            private readonly Vector2 centerLeft;
            private readonly Vector2 topCenter;
            private readonly Vector2 bottomCenter;
            private readonly Vector2 rightCenter;

            private readonly QuadTreeNode<T>[] nodes;

            private readonly List<T> children;

            public QuadTreeNode(QuadTree<T> parent, Vector2 bottomLeft, Vector2 topRight, int height)
            {
                this.parent = parent;
                this.bottomLeft = bottomLeft;
                this.topRight = topRight;
                this.center = (this.bottomLeft + this.topRight) / 2;
                this.centerLeft = new Vector2(this.bottomLeft.X, this.center.Y);
                this.topCenter = new Vector2(this.center.X, this.topRight.Y);
                this.bottomCenter = new Vector2(this.center.X, this.bottomLeft.Y);
                this.rightCenter = new Vector2(this.topRight.X, this.center.Y);

                this.height = height;

                this.nodes = new QuadTreeNode<T>[4];
                this.children = new List<T>();
            }

            public void PushItem(T item)
            {
                var currentIntersection = item.CheckBounding(this.bottomLeft, this.topRight);
                if(currentIntersection != BoundingType.Contains)
                {
                    throw new ArgumentException($"Cannot add item to the quadtree because it does not intersect the current tree node.");
                }

                // Short circuit on the max height.
                if (this.height >= this.parent.maxHeight)
                {
                    this.children.Add(item);
                    return;
                }

                for (var i = 0; i < 4; i++)
                {
                    this.GetBoundsForIndex(i, out var min, out var max);

                    if (item.CheckBounding(min, max) == BoundingType.Contains)
                    {
                        var childNode = this.GetOrCreateNode(i);
                        childNode.PushItem(item);
                        return;
                    }
                }

                this.children.Add(item);
            }

            public IEnumerable<T> IterateRange(Vector2 min, Vector2 max)
            {
                for(int i = 0; i < 2; i++)
                {
                    if (this.nodes[i] == null)
                    {
                        continue;
                    }

                    this.GetBoundsForIndex(i, out var cmin, out var cmax);

                    var bType = Vector2.BoundingBox(min, max, cmin, cmax);

                    if (bType == BoundingType.Contains
                        || bType == BoundingType.Intersects)
                    {
                        foreach (var t in this.nodes[i].IterateRange(min, max))
                        {
                            yield return t;
                        }
                    }
                }

                foreach(var item in this.children)
                {
                    if (item.CheckBounding(min, max) != BoundingType.Disjoint)
                    {
                        yield return item;
                    }
                }

                for (int i = 2; i < 4; i++)
                {
                    if (this.nodes[i] == null)
                    {
                        continue;
                    }

                    this.GetBoundsForIndex(i, out var cmin, out var cmax);

                    var bType = Vector2.BoundingBox(min, max, cmin, cmax);

                    if (bType == BoundingType.Contains
                        || bType == BoundingType.Intersects)
                    {
                        foreach (var t in this.nodes[i].IterateRange(min, max))
                        {
                            yield return t;
                        }
                    }
                }
            }

            /// <summary>
            /// Get the bounds for each index.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            private void GetBoundsForIndex(int index, out Vector2 min, out Vector2 max)
            {
                switch(index)
                {
                    case 0:
                        min = this.centerLeft;
                        max = this.topCenter;
                        break;
                    case 1:
                        min = this.center;
                        max = this.topRight;
                        break;
                    case 2:
                        min = this.bottomCenter;
                        max = this.rightCenter;
                        break;
                    case 3:
                        min = this.bottomLeft;
                        max = this.center;
                        break;
                    default:
                        throw new ArgumentException("Index must be less than 4.");
                }
            }

            /// <summary>
            /// Gets or creates a node a the index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>The node.</returns>
            private QuadTreeNode<T> GetOrCreateNode(int index)
            {
                if (null == this.nodes[index])
                {
                    this.GetBoundsForIndex(index, out var min, out var max);
                    this.nodes[index] = new QuadTreeNode<T>(this.parent, min, max, this.height + 1);
                }

                return this.nodes[index];
            }
        }
    }
}
