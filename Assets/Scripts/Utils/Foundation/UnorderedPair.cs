using System;
using UnityEngine;

namespace TX
{
    public class UnorderedPair<T>
    {
        public readonly T A;
        public readonly T B;

        public UnorderedPair(T a, T b)
        {
            if (a == null || b == null)
            {
                throw new NullReferenceException();
            }
            A = a;
            B = b;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            UnorderedPair<T> p = obj as UnorderedPair<T>;
            if (p == null)
                return false;
            return (p.A.Equals(A) && p.B.Equals(B)) ||
                (p.B.Equals(A) && p.A.Equals(B));
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode();
        }

        public override string ToString()
        {
            return "{" + A.ToString() + ", " + B.ToString() + "}";
        }
    }
}
