using System;
using System.Collections.Generic;
using System.Linq;

namespace TX
{
    [Serializable]
    public class Range<T>
    {
        public T min;
        public T max;

        public Range(T min, T max)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            return string.Format("{0} ~ {1}", min, max);
        }
    }

    [Serializable]
    public class Rangei : Range<int>
    {
        public int median { get { return min + (max - min) / 2; } }

        public Rangei(int min, int max) : base(min, max) { }

        public bool Contains(int val)
        {
            return min <= val && val <= max;
        }
    }

    [Serializable]
    public class Rangef : Range<float>
    {
        public float median { get { return (min + max) / 2; } }

        public Rangef(float min, float max) : base(min, max) { }

        public bool Contains(float val)
        {
            return min <= val && val <= max;
        }
    }
}
