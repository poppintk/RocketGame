using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TX
{
    public class BTNode<T> : IEnumerable<T>
    {
        private BTNode<T>[] _children;

        public T Value { get; set; }
        public BTNode<T> Parent { get; private set; }

        public BTNode<T> Left
        {
            get { return _children[0]; }
            set { this[0] = value; }
        }

        public BTNode<T> Right
        {
            get { return _children[1]; }
            set { this[1] = value; }
        }

        public BTNode<T> this[int i]
        {
            get
            {
                return _children[i];
            }
            set
            {
                _children[i] = value;
                value.Parent = this;
            }
        }

        public bool IsLeaf { get { return _children[0] == null && _children[1] == null; } }

        public BTNode(T value, BTNode<T> left = null, BTNode<T> right = null)
        {
            Value = value;
            _children = new BTNode<T>[2] { left, right };
        }

        public IEnumerator<T> GetEnumerator()
        {
            return DepthFirst().Select(n => n.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<BTNode<T>> DepthFirst()
        {
            Stack<BTNode<T>> todo = new Stack<BTNode<T>>();
            todo.Push(this);
            while (todo.Count > 0)
            {
                var curr = todo.Pop();
                yield return curr;
                if (curr.Left != null)
                    todo.Push(curr.Left);
                if (curr.Right != null)
                    todo.Push(curr.Right);
            }
        }

        public IEnumerable<BTNode<T>> BreadthFirst()
        {
            Queue<BTNode<T>> todo = new Queue<BTNode<T>>();
            todo.Enqueue(this);
            while (todo.Count > 0)
            {
                var curr = todo.Dequeue();
                yield return curr;
                if (curr.Left != null)
                    todo.Enqueue(curr.Left);
                if (curr.Right != null)
                    todo.Enqueue(curr.Right);
            }
        }

        public IEnumerable<BTNode<T>> GetAllLeaves()
        {
            return DepthFirst().Where(n => n.Left == null && n.Right == null);
        }
    }
}
