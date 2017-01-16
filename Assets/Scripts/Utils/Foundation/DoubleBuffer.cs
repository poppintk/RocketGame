using System;
using System.Collections.Generic;

namespace TX
{
    public class DoubleBuffer<T> where T : class, new()
    {
        /// <summary>The current buffer.</summary>
        public T Curr
        {
            get { return buf[currIdx]; }
            private set { buf[currIdx] = value; }
        }

        /// <summary>The next buffer.</summary>
        public T Next
        {
            get { return buf[1 - currIdx]; }
            private set { buf[1 - currIdx] = value; }
        }

        private int currIdx = 0;
        private T[] buf = new T[2];

        public DoubleBuffer()
        {
            Curr = new T();
            Next = new T();
        }

        /// <summary>Switches the current and the next buffer.</summary>
        public void SwitchBuffers()
        {
            currIdx = 1 - currIdx;
        }
    }
}
