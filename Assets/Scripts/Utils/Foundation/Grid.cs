using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TX
{
    public abstract class Grid
    {
        public readonly Vec2i Size;
        public int Width { get { return Size.x; } }
        public int Height { get { return Size.y; } }
        public readonly Recti LocalRegion;
        public readonly int TotalCells;

        public Grid(int w, int h)
        {
            Size = new Vec2i(w, h);
            LocalRegion = new Recti(0, 0, w, h);
            TotalCells = w * h;
        }

        public override string ToString()
        {
            return string.Format("{0}({1}, {2})", GetType().Name, Width, Height);
        }
    }

    /// <summary>
    /// 2D generic array wrapper.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Grid<T> : Grid, IEnumerable<T>
    {
        public readonly T[,] data;

        public T this[int x, int y]
        {
            get { return data[x, y]; }
            set { data[x, y] = value; }
        }

        public T this[Vec2i v]
        {
            get { return data[v.x, v.y]; }
            set { data[v.x, v.y] = value; }
        }

        public Grid(int w, int h) : base(w, h)
        {
            data = new T[w, h];
        }

        public Grid(int wh) : this(wh, wh)
        {
        }

        public Grid(T[,] array) : base(array.GetLength(0), array.GetLength(1))
        {
            data = new T[Width, Height];
            Array.Copy(array, data, array.Length);
        }

        /// <summary>Initializes all cells with a class that has default constructor.</summary>
        /// <typeparam name="S"></typeparam>
        public void Init<S>() where S : T, new()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    data[x, y] = new S();
        }

        public void Clear()
        {
            Array.Clear(data, 0, data.Length);
        }

        public void Clear(T val)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    data[x, y] = val;
        }

        /// <summary>
        /// Tries to get the value of an element in the grid.
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <param name="v"> The coord. </param>
        /// <param name="value"> The value. </param>
        /// <returns> True if succeeded. </returns>
        public bool TryGet(Vec2i v, out T value)
        {
            if (LocalRegion.Contains(v))
            {
                value = data[v.x, v.y];
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return data.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(this[x, y]);
                    sb.Append('\t');
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// A grid of boolean value.
    /// </summary>
    /// <seealso cref="TX.Grid" />
    public class BitGrid : Grid
    {
        protected readonly BitArray data;

        public bool this[int x, int y]
        {
            get { return data[y * Width + x]; }
            set { data[y * Width + x] = value; }
        }

        public bool this[Vec2i v]
        {
            get { return data[v.y * Width + v.x]; }
            set { data[v.y * Width + v.x] = value; }
        }

        public BitGrid(int w, int h) : base(w, h)
        {
            data = new BitArray(w * h);
        }

        public BitGrid(BitGrid ot) : base(ot.Width, ot.Height)
        {
            data = new BitArray(ot.data);
        }

        public void SetAll(bool v)
        {
            data.SetAll(v);
        }

        public void And(BitGrid ot)
        {
            MustCompatible(this, ot);
            data.And(ot.data);
        }

        public void Or(BitGrid ot)
        {
            MustCompatible(this, ot);
            data.Or(ot.data);
        }

        public void Not()
        {
            data.Not();
        }

        public void Xor(BitGrid ot)
        {
            MustCompatible(this, ot);
            data.Xor(ot.data);
        }

        public int CountBits()
        {
            return data.GetCardinality();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(this[x, y] ? '1' : '0');
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private static void MustCompatible(BitGrid a, BitGrid b)
        {
            if (a.Width != b.Width || a.Height != b.Height)
            {
                throw new ArgumentException("Incompatible bit grids!");
            }
        }
    }

    /// <summary>
    /// A grid of element of type <see cref="T"/> with one bit associate to each cell.
    /// </summary>
    /// <seealso cref="TX.BitGrid" />
    public class BitGrid<T> : BitGrid
    {
        public readonly Grid<T> Items;

        public BitGrid(int w, int h) : base(w, h)
        {
            Items = new Grid<T>(w, h);
        }

        public BitGrid(BitGrid ot) : base(ot)
        {
            Items = new Grid<T>(ot.Width, ot.Height);
        }
    }
}
