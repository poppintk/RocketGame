using System;

namespace TX
{
    /// <summary>
    /// WELL-512 (better and faster than MT19937)
    /// http://www.lomont.org/Math/Papers/2008/Lomont_PRNG_2008.pdf
    /// </summary>
    public class RNG
    {
        /// <summary>The initial seed (for bookeeping only). </summary>
        public readonly int InitialSeed;

        public class State
        {
            public readonly uint[] svec = new uint[16];
            public uint index;

            public State(int seed)
            {
                index = 0;
                Random random = new Random(seed);
                for (int i = 0; i < 16; i++)
                {
                    svec[i] = (uint)random.Next();
                }
            }

            public State(State other)
            {
                Array.Copy(svec, other.svec, 16);
                index = other.index;
            }
        }

        public State state;

        public RNG(int seed)
        {
            InitialSeed = seed;
            state = new State(seed);
        }

        public RNG(State state)
        {
            this.state = new State(state);
        }

        internal uint Next(int minValue, int maxValue)
        {
            return (uint)((NextUInt() % (maxValue - minValue)) + minValue);
        }

        public uint Next(uint maxValue)
        {
            return NextUInt() % maxValue;
        }

        public uint NextUInt()
        {
            uint a, b, c, d;

            a = state.svec[state.index];
            c = state.svec[(state.index + 13) & 15];
            b = a ^ c ^ (a << 16) ^ (c << 15);
            c = state.svec[(state.index + 9) & 15];
            c ^= (c >> 11);
            a = state.svec[state.index] = b ^ c;
            d = a ^ ((a << 5) & 0xda442d24U);
            state.index = (state.index + 15) & 15;
            a = state.svec[state.index];
            state.svec[state.index] = a ^ b ^ d ^ (a << 2) ^ (b << 18) ^ (c << 28);

            return state.svec[state.index];
        }

        /// <summary> Integer on [-2^31, 2^31-1] </summary>
        public int NextInt() { return (int)NextUInt(); }

        /// <summary> Integer on [0, N-1] </summary>
        public int NextIntRange(int iN) { return (int)(NextUInt() * (iN / 4294967296f)); }

        /// <summary> Integer on [A, B-1] </summary>
        public int NextIntRange(int iA, int iB) { return NextIntRange(iB - iA) + iA; }

        /// <summary> Float on [0,1] </summary>
        public float NextFloat1() { return NextUInt() * (1f / 4294967295f); }

        /// <summary> Float on [0,1) </summary>
        public float NextFloat() { return NextUInt() * (1f / 4294967296f); }

        /// <summary> Float on [min, max] </summary>
        public float NextFloatRange(float min, float max)
        {
            float f = NextFloat1();
            return min + f * (max - min);
        }

        public bool NextBool(float oddTrue = 0.5f) { return NextFloat() < oddTrue; }

        public Vec2i NextVec2i(Recti rect)
        {
            return new Vec2i(
                rect.left + NextIntRange(rect.width),
                rect.bottom + NextIntRange(rect.height));
        }
    }
}
