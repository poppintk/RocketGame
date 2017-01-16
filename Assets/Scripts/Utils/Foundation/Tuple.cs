using System;

namespace TX
{
    /// <summary>
    /// Static tuple methods. 
    /// </summary>
    public static class Tuple
    {
        public static Tuple<T1, T2> New<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }
    }

    /// <summary>
    /// Tuple of two objects. 
    /// </summary>
    /// <typeparam name="T1"> The type of the item 1. </typeparam>
    /// <typeparam name="T2"> The type of the item 2. </typeparam>
    [Serializable]
    public class Tuple<T1, T2>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }

        internal Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override string ToString()
        {
            return string.Format("<{0}, {1}>", Item1, Item2);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Item1.GetHashCode();
            hash = hash * 23 + Item2.GetHashCode();
            return hash;
        }
    }
}
