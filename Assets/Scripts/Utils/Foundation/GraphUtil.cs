using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TX
{
    public interface IRect
    {
        float Area { get; }
        Rect Bound { get; set; }
    }

    public interface IDAGNode
    {
        object NodeValue { get; }
        IEnumerable<IDAGNode> Destinations { get; }
    }

    public interface IDAGNode<T> : IDAGNode
    {
        T Value { get; }
    }

    public static class IDAGNodeExt
    {
        public static bool CanReach(this IDAGNode origin, IDAGNode dest)
        {
            return origin.BFS(dest.Equals) != null;
        }

        public static IDAGNode BFS(this IDAGNode origin, Func<IDAGNode, bool> match)
        {
            Queue<IDAGNode> queue = new Queue<IDAGNode>();
            queue.Enqueue(origin);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (match(node))
                {
                    return node;
                }
                foreach (var dest in node.Destinations)
                {
                    queue.Enqueue(dest);
                }
            }
            return null;
        }

        public static IDAGNode DFS(this IDAGNode origin, Func<IDAGNode, bool> match)
        {
            Stack<IDAGNode> stack = new Stack<IDAGNode>();
            stack.Push(origin);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (match(node))
                {
                    return node;
                }
                foreach (var dest in node.Destinations)
                {
                    stack.Push(dest);
                }
            }
            return null;
        }

        public static IEnumerable<IDAGNode<T>> GetAccessibleNodes<T>(this IDAGNode<T> root)
        {
            Queue<IDAGNode<T>> queue = new Queue<IDAGNode<T>>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                foreach (var dest in node.Destinations)
                {
                    queue.Enqueue((IDAGNode<T>)dest);
                }
                yield return node;
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("[{Count}] {Value}")]
    public class NTree<T> : IDAGNode<T>
    {
        public NTree<T> Parent { get; private set; }

        public T Value { get; set; }

        private List<NTree<T>> _children = new List<NTree<T>>();

        public int Count { get { return _children.Count; } }

        public bool IsLeaf { get { return _children.Count == 0; } }

        public IEnumerable<NTree<T>> Children { get { return _children; } }

        object IDAGNode.NodeValue { get { return Value; } }

        IEnumerable<IDAGNode> IDAGNode.Destinations
        {
            get { return _children.Cast<IDAGNode>(); }
        }

        public NTree(T data)
        {
            Value = data;
        }

        protected NTree(T data, NTree<T> parent) : this(data)
        {
            Parent = parent;
        }

        protected NTree<T> AddChild(NTree<T> child)
        {
            _children.Add(child);
            return child;
        }

        /// <summary>Adds a child.</summary>
        /// <param name="data">The containing data.</param>
        /// <returns>The child node.</returns>
        public virtual NTree<T> AddChild(T data)
        {
            return AddChild(new NTree<T>(data, this));
        }

        /// <summary>Removes this node from the parent.</summary>
        public virtual void Remove()
        {
            if (Parent != null)
            {
                Parent._children.Remove(this);
            }
        }

        /// <summary>Pre-order traversal.</summary>
        public void PreOrder(Action<NTree<T>> visit)
        {
            visit(this);
            foreach (var child in _children)
                child.PreOrder(visit);
        }

        /// <summary>Post-order traversal.</summary>
        public void PostOrder(Action<NTree<T>> visit)
        {
            foreach (var child in _children)
                child.PostOrder(visit);
            visit(this);
        }
    }

    [System.Diagnostics.DebuggerDisplay("{Value.Bound}[{AreaSum}]")]
    public class RectTree<T> : NTree<T> where T : IRect
    {
        public float AreaSum { get; private set; }

        public RectTree(T data) : base(data)
        {
            AreaSum = data.Area;
        }

        private RectTree(T data, RectTree<T> parent) : base(data, parent)
        {
            AreaSum = data.Area;
        }

        public override NTree<T> AddChild(T data)
        {
            var child = AddChild(new RectTree<T>(data, this));
            UpdateAreaSum();
            return child;
        }

        public override void Remove()
        {
            base.Remove();
            UpdateAreaSum();
        }

        /// <summary>Recalculates the area sum for the current node.</summary>
        public void RecalculateAreaSum()
        {
            AreaSum = Value.Area + Children.Cast<RectTree<T>>().Sum(n => n.AreaSum);
        }

        /// <summary>Updates the area sum for the entire tree.</summary>
        public void UpdateAreaSumTree()
        {
            PostOrder(n => ((RectTree<T>)n).RecalculateAreaSum());
        }

        /// <summary>Bottom-up update of the area sum.</summary>
        private void UpdateAreaSum()
        {
            var node = this;
            while (node != null)
            {
                node.RecalculateAreaSum();
                node = (RectTree<T>)node.Parent;
            }
        }
    }
}
