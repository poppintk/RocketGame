using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace TX
{
    public static class MathUtil
    {
        /// <summary>
        /// Modulo function (always return non-negatives).
        /// NOTE: '%' is not modulo but remainder (same sign as divdend).
        /// </summary>
        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        /// <summary>Gets all the divisors unordered. (excluding 1 and itself).</summary>
        public static IEnumerable<int> GetDivisors(int number)
        {
            int i = 2;
            int sqrt = (int)Math.Sqrt(number);
            List<int> result = new List<int>();
            while (i <= sqrt)
            {
                if (number % i == 0)
                {
                    result.Add(i);
                    if (i != number / i)
                    {
                        result.Add(number / i);
                    }
                }
                i += 1;
            }
            return result;
        }

        /// <summary>
        /// Inclusive range comparison.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum inclusive.</param>
        /// <param name="max">The maximum inclusive.</param>
        /// <returns></returns>
        public static bool InRange<T>(T value, T min, T max) where T : IComparable<T>
        {
            return min.CompareTo(value) <= 0 && value.CompareTo(max) <= 0;
        }

        public static Vec2i FloorToInt(Vector2 v)
        {
            return new Vec2i(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        public static Vec2i RoundToInt(Vector2 v)
        {
            return new Vec2i(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static int ToPercent(float f) { return (int)(f * 100); }
    }

    [Serializable]
    public struct Vec2i
    {
        public int x;

        public int y;

        public Vec2i yx { get { return new Vec2i(y, x); } }

        public int this[int i]
        {
            get
            {
                return i == 0 ? x : y;
            }
            set
            {
                if (i == 0)
                    x = value;
                else
                    y = value;
            }
        }

        public float length { get { return Mathf.Sqrt(x * x + y * y); } }

        public static implicit operator Vector2(Vec2i v) { return new Vector2(v.x, v.y); }

        public static implicit operator Vector3(Vec2i v) { return new Vector3(v.x, v.y); }

        public static explicit operator Dir(Vec2i v) { return vec2iToDir[v]; }

        public Vec2i(int xy) { this.x = this.y = xy; }

        public Vec2i(int x, int y) { this.x = x; this.y = y; }

        public Vec2i(Vector2 v) { this.x = (int)v.x; this.y = (int)v.y; }

        /// <summary> (0, -1) </summary>
        public static readonly Vec2i down = new Vec2i(0, -1);

        /// <summary> (-1, 0) </summary>
        public static readonly Vec2i left = new Vec2i(-1, 0);

        /// <summary> (1, 1) </summary>
        public static readonly Vec2i one = new Vec2i(1, 1);

        /// <summary> (1, 0) </summary>
        public static readonly Vec2i right = new Vec2i(1, 0);

        /// <summary> (0, 1) </summary>
        public static readonly Vec2i up = new Vec2i(0, 1);

        /// <summary> (0, 0) </summary>
        public static readonly Vec2i zero = new Vec2i(0, 0);

        private static readonly Dictionary<Vec2i, Dir> vec2iToDir = new Dictionary<Vec2i, Dir>
        {
            { new Vec2i(0, -1), Dir.S },
            { new Vec2i(-1, 0), Dir.W },
            { new Vec2i(1, 0), Dir.E },
            { new Vec2i(0, 1), Dir.N },
        };

        public static Vec2i operator -(Vec2i a) { return new Vec2i(-a.x, -a.y); }

        public static Vec2i operator +(Vec2i a) { return a; }

        public static Vec2i operator +(Vec2i c, int i) { return new Vec2i(c.x + i, c.y + i); }

        public static Vec2i operator -(Vec2i c, int i) { return new Vec2i(c.x - i, c.y - i); }

        public static Vec2i operator *(Vec2i c, int i) { return new Vec2i(c.x * i, c.y * i); }

        public static Vec2i operator /(Vec2i c, int i) { return new Vec2i(c.x / i, c.y / i); }

        public static Vec2i operator +(Vec2i a, Vec2i b) { return new Vec2i(a.x + b.x, a.y + b.y); }

        public static Vec2i operator -(Vec2i a, Vec2i b) { return new Vec2i(a.x - b.x, a.y - b.y); }

        public static Vec2i operator *(Vec2i a, Vec2i b) { return new Vec2i(a.x * b.x, a.y * b.y); }

        public static Vec2i operator /(Vec2i a, Vec2i b) { return new Vec2i(a.x / b.x, a.y / b.y); }

        public static bool operator <(Vec2i a, Vec2i b) { return a.x < b.x && a.y < b.y; }

        public static bool operator >(Vec2i a, Vec2i b) { return a.x > b.x && a.y > b.y; }

        public static bool operator <=(Vec2i a, Vec2i b) { return a.x <= b.x && a.y <= b.y; }

        public static bool operator >=(Vec2i a, Vec2i b) { return a.x >= b.x && a.y >= b.y; }

        public static bool operator ==(Vec2i a, Vec2i b) { return a.x == b.x && a.y == b.y; }

        public static bool operator !=(Vec2i a, Vec2i b) { return a.x != b.x && a.y != b.y; }

        /// <summary>
        /// Returns the 8 moore neighbours starting from east.
        /// </summary>
        public IEnumerable<Vec2i> MooreNeighbours()
        {
            Vec2i cell = new Vec2i(x + 1, y);
            yield return cell;              // E
            cell.y++; yield return cell;    // NE
            cell.x--; yield return cell;    // N
            cell.x--; yield return cell;    // NW
            cell.y--; yield return cell;    // W
            cell.y--; yield return cell;    // SW
            cell.x++; yield return cell;    // S
            cell.x++; yield return cell;    // SE
        }

        /// <summary>
        /// Returns the 4 von Neumann neighbours.
        /// </summary>
        public IEnumerable<Vec2i> VonNeumannNeighbours()
        {
            yield return new Vec2i(x + 1, y);   // E
            yield return new Vec2i(x, y + 1);   // N
            yield return new Vec2i(x - 1, y);   // W
            yield return new Vec2i(x, y - 1);   // S
        }

        /// <summary> Returns the cells that are some distance away from current cell.</summary>
        public IEnumerable<Vec2i> MooreRing(int distance)
        {
            if (distance < 0)
                throw new ArgumentOutOfRangeException("distance");
            if (distance == 0)
            {
                yield return this;
            }
            else
            {
                foreach (Dir side in EnumUtil.GetValues<Dir>())
                {
                    Vec2i curr = Go(side.ToDir8().NextCW(), distance);
                    Dir growDir = side.NextCCW();

                    for (int s = distance * 2; s > 0; s--)
                    {
                        yield return curr;
                        curr = curr.Go(growDir);
                    }
                }
            }
        }

        public Vec2i Go(Dir dir, int dist = 1)
        {
            return this + dir.ToVec2i() * dist;
        }

        public Vec2i Go(Dir8 dir, int dist = 1)
        {
            return this + dir.ToVec2i() * dist;
        }

        public Vector3 ToXY(float z = 0)
        {
            return new Vector3(x, y, z);
        }

        public Vector3 ToXZ(float y = 0)
        {
            return new Vector3(x, y, this.y);
        }

        //public int CompareTo(Vec2i ot)
        //{
        //    return x.CompareTo(ot.x) == y.CompareTo(ot.y) ? x.CompareTo(ot.x) : -1;
        //}

        public override string ToString()
        {
            return string.Format("({0}, {1})", x, y);
        }

        public override bool Equals(object obj)
        {
            return (obj is Vec2i) && ((Vec2i)obj) == this;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = ((hash + x) << 5) - (hash + x);
            hash = ((hash + y) << 5) - (hash + y);
            return hash;
        }
    }

    public abstract class SegmentBaseInt<T>
    {
        public T a { get; set; }
        public T b { get; set; }
        public abstract int length { get; }
        public abstract T center { get; }

        public SegmentBaseInt(T a, T b) { this.a = a; this.b = b; }
    }

    public class Segment1i : SegmentBaseInt<int>, IEnumerable<int>
    {
        public override int length { get { return b - a; } }
        public override int center { get { return (a + b) / 2; } }

        public Segment1i(int a, int b) : base(a, b)
        {
            if (a > b)
                throw new ArgumentException();
        }

        public bool Overlaps(Segment1i other)
        {
            return a < other.b && b > other.a;
        }

        /// <summary>Returns the distance (non-negative) to another segment.</summary>
        /// <param name="other">The other segment.</param>
        public int Distance(Segment1i other)
        {
            return Overlaps(other) ? 0 :
                Math.Min(Math.Abs(a - other.b + 1), Math.Abs(other.a - b + 1));
        }

        //public Segment1i Resize(int amount)
        //{
        //    Assert.IsTrue(length + amount * 2 > 0);
        //    if (!incremental)
        //        amount = -amount;

        //    a -= amount;
        //    b += amount;

        //    return this;
        //}

        public Segment1i Intersect(Segment1i other)
        {
            return Overlaps(other) ?
                new Segment1i(Math.Max(a, other.a), Math.Min(b, other.b)) :
                new Segment1i(0, 0);
        }

        public Segment1i Union(Segment1i other)
        {
            return new Segment1i(Math.Min(a, other.a), Math.Max(b, other.b));
        }

        public IEnumerable<Segment1i> Lerp(Segment1i target)
        {
            if (target.a == a && target.b == b)
                yield break;
            int currA = a,
                currB = b,
                lenA = target.a - a,
                lenB = target.b - b,
                stepA = Math.Sign(lenA),
                stepB = Math.Sign(lenB);
            for (int i = Math.Max(Math.Abs(lenA), Math.Abs(lenB)); i > 1; i--)
            {
                if (currA != target.a)
                    currA += stepA;
                if (currB != target.b)
                    currB += stepB;
                yield return new Segment1i(currA, currB);
            }
        }

        #region Interface/Overriden Methods

        public override string ToString()
        {
            return string.Format("{0}, {1}", a, b);
        }

        private IEnumerator<int> GetEnumerator()
        {
            for (int i = a; i < b; i++)
                yield return i;
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    public class Segment2i : SegmentBaseInt<Vec2i>
    {
        public override Vec2i center { get { return (a + b) / 2; } }
        public override int length { get { throw new NotImplementedException(); } }

        public Segment2i(Vec2i a, Vec2i b) : base(a, b) { }
    }

    public class RectiSide : Segment1i
    {
        public Dir dir;
        public int otherAxis;

        public IEnumerable<Vec2i> cells { get { return ((IEnumerable<int>)this).Select(GetIntToCellConverter()); } }

        public RectiSide(Dir dir, int a, int b, int otherAxis) : base(a, b)
        {
            this.dir = dir;
            this.otherAxis = otherAxis;
        }

        public RectiSide(RectiSide ot) : this(ot.dir, ot.a, ot.b, ot.otherAxis) { }

        /// <summary>Gets the converter from 1D position to 2D cell on this side.</summary>
        /// <returns>The converter. </returns>
        public Func<int, Vec2i> GetIntToCellConverter()
        {
            if (dir.IsVertical())
                return x => new Vec2i(x, otherAxis);
            else
                return y => new Vec2i(otherAxis, y);
        }

        public override string ToString()
        {
            return base.ToString() + ", " + dir.ToString() + ", " + otherAxis.ToString();
        }
    }

    [Serializable]
    public struct Recti : IEnumerable<Vec2i>
    {
        public Vec2i position { get { return _pos; } set { _pos = value; } }

        [SerializeField]
        private Vec2i _pos;

        public Vec2i size { get { return _size; } set { _size = value; } }

        [SerializeField]
        private Vec2i _size;

        public int width { get { return _size.x; } set { _size.x = value; } }
        public int height { get { return _size.y; } set { _size.y = value; } }

        public Vec2i min { get { return _pos; } set { _pos = min; } }
        public Vec2i max { get { return _pos + _size; } set { _size = value - _pos; } }

        public int this[Dir dir]
        {
            get
            {
                switch (dir)
                {
                    case Dir.E: return xMax - 1;
                    case Dir.N: return yMax - 1;
                    case Dir.S: return yMin;
                    case Dir.W: return xMin;
                    default: throw new IndexOutOfRangeException("Unknown direction.");
                }
            }
        }

        public int yMax { get { return position.y + size.y; } set { _size.y = value - _pos.y; } }
        public int top { get { return yMax; } }
        public int xMin { get { return position.x; } set { _size.x += _pos.x - value; _pos.x = value; } }
        public int left { get { return xMin; } }
        public int xMax { get { return position.x + size.x; } set { _size.x = value - _pos.x; } }
        public int right { get { return xMax; } }
        public int yMin { get { return position.y; } set { _size.y += _pos.y - value; _pos.y = value; } }
        public int bottom { get { return yMin; } }

        public Vec2i topLeft { get { return new Vec2i(left, top - 1); } }
        public Vec2i topRight { get { return new Vec2i(right - 1, top - 1); } }
        public Vec2i bottomLeft { get { return new Vec2i(left, bottom); } }
        public Vec2i bottomRight { get { return new Vec2i(right - 1, bottom); } }
        public Vector2 center { get { return position + (Vector2)size / 2f; } }
        public Vec2i centeri { get { return position + size / 2; } }

        public float aspectRatio { get { return (float)width / height; } }

        public int area { get { return width * height; } }

        public static implicit operator Rect(Recti r) { return new Rect(r.position, r.size); }

        public Recti(Vec2i pos, Vec2i size) { _pos = pos; _size = size; }

        public Recti(Recti other) { _pos = other._pos; _size = other._size; }

        public Recti(int xmin, int ymin, int width, int height)
        {
            _pos = new Vec2i(xmin, ymin);
            _size = new Vec2i(width, height);
        }

        public static bool operator ==(Recti a, Recti b) { return a.position == b.position && a.size == b.size; }

        public static bool operator !=(Recti a, Recti b) { return a.position != b.position || a.size != b.size; }

        public static Recti MinMaxRecti(int xmin, int ymin, int xmax, int ymax)
        {
            return new Recti(xmin, ymin, Mathf.Max(0, xmax - xmin), Mathf.Max(0, ymax - ymin));
        }

        /// <summary>
        /// Gets the corner position CCW after given side.
        /// </summary>
        public Vec2i GetCornerAfter(Dir side)
        {
            switch (side)
            {
                case Dir.E:
                    return topRight;
                case Dir.N:
                    return topLeft;
                case Dir.W:
                    return bottomLeft;
                case Dir.S:
                    return bottomRight;
                default:
                    return new Vec2i(); // shouldn't hit here
            }
        }

        /// <summary>Get one side of the rectangle, the tiles are in CCW order.</summary>
        /// <param name="dir">The direction.</param>
        /// <param name="skipTail">Whether to skip the last one, this can be useful for looping all edge tiles.</param>
        /// <exception cref="System.ArgumentException">Unknown direction</exception>
        public RectiSide GetSideOn(Dir dir, bool skipTail = false)
        {
            switch (dir)
            {
                case Dir.E:
                    return new RectiSide(dir, yMin, skipTail ? yMax - 1 : yMax, xMax - 1);
                case Dir.N:
                    return new RectiSide(dir, skipTail ? xMin + 1 : xMin, xMax, yMax - 1);
                case Dir.S:
                    return new RectiSide(dir, xMin, skipTail ? xMax - 1 : xMax, yMin);
                case Dir.W:
                    return new RectiSide(dir, skipTail ? yMin + 1 : yMin, yMax, xMin);
                default:
                    throw new ArgumentException("Unknown direction");
            }
        }

        public bool Contains(Vec2i p)
        {
            Vec2i pLocal = p - position;
            return MathUtil.InRange(pLocal.x, 0, size.x - 1) && MathUtil.InRange(pLocal.y, 0, size.y - 1);
        }

        public bool Contains(Recti r) { return min <= r.min && max >= r.max; }

        public bool Overlaps(Recti r) { return max > r.min && min < r.max; }

        public Recti Move(Vec2i offset) { return new Recti(position + offset, size); }

        public Recti MoveTo(Vec2i pos) { return new Recti(pos, size); }

        /// <summary> Get the intersection region with the specified rectangle. </summary>
        public Recti Intersect(Recti ot)
        {
            return MinMaxRecti(
                Math.Max(xMin, ot.xMin),
                Math.Max(yMin, ot.yMin),
                Math.Min(xMax, ot.xMax),
                Math.Min(yMax, ot.yMax));
        }

        /// <summary> Get the unioned region with the specified rectangle. </summary>
        public Recti Union(Recti ot)
        {
            return MinMaxRecti(
                Math.Min(xMin, ot.xMin),
                Math.Min(yMin, ot.yMin),
                Math.Max(xMax, ot.xMax),
                Math.Max(yMax, ot.yMax));
        }

        public Recti Expand(Vec2i th) { return MinMaxRecti(min.x - th.x, min.y - th.y, max.x + th.x, max.y + th.y); }

        public Recti Expand(Dir side, int thickness)
        {
            switch (side)
            {
                case Dir.E: return MinMaxRecti(xMin, yMin, xMax + thickness, yMax);
                case Dir.S: return MinMaxRecti(xMin, yMin - thickness, xMax, yMax);
                case Dir.N: return MinMaxRecti(xMin, yMin, xMax, yMax + thickness);
                case Dir.W: return MinMaxRecti(xMin - thickness, yMin, xMax, yMax);
                default: throw new ArgumentException();
            }
        }

        /// <summary>Expands to include a point.</summary>
        /// <param name="v">The v.</param>
        public void ExpandToInclude(Vec2i v)
        {
            if (width == 0 || height == 0)
            {
                position = v;
                size = Vec2i.one;
            }
            else
            {
                if (v.x < xMin)
                    xMin = v.x;
                else if (v.x >= xMax)
                    xMax = v.x + 1;
                if (v.y < yMin)
                    yMin = v.y;
                else if (v.y >= yMax)
                    yMax = v.y + 1;
            }
        }

        public Recti Shrink(Vec2i thickness) { return MinMaxRecti(xMin + thickness.x, yMin + thickness.y, xMax - thickness.x, yMax - thickness.y); }

        public Recti Shrink(Dir side, int thickness)
        {
            switch (side)
            {
                case Dir.E: return MinMaxRecti(xMin, yMin, xMax - thickness, yMax);
                case Dir.S: return MinMaxRecti(xMin, yMin + thickness, xMax, yMax);
                case Dir.N: return MinMaxRecti(xMin, yMin, xMax, yMax - thickness);
                case Dir.W: return MinMaxRecti(xMin + thickness, yMin, xMax, yMax);
                default: throw new ArgumentException();
            }
        }

        public Recti[] Divide(bool verticalCut, int line)
        {
            if (verticalCut)
            {
                return MathUtil.InRange(line, left + 1, right - 1) ?
                    new[]
                    {
                        MinMaxRecti(left, bottom, line, top),
                        MinMaxRecti(line, bottom, right, top)
                    } : new[] { this };
            }
            else
            {
                return MathUtil.InRange(line, bottom + 1, top - 1) ?
                    new[]
                    {
                        MinMaxRecti(left, bottom, right, line),
                        MinMaxRecti(left, line, right, top)
                    } : new[] { this };
            }
        }

        public Recti[] MultiCut(bool verticalCut, int[] lines)
        {
            List<Recti> regions = new List<Recti>();
            Array.Sort(lines);
            int cutAxis = Convert.ToInt32(!verticalCut);
            int cutMax = max[cutAxis];
            int cutMin = min[cutAxis];

            Vec2i currPos = position;
            Vec2i currSize = size;
            currSize[cutAxis] = lines[0] - cutMin;

            regions.Add(new Recti(currPos, currSize));
            for (int i = 0; i < lines.Length; i++)
            {
                int nextLine = i < lines.Length - 1 ? lines[i + 1] : cutMax;
                currPos[cutAxis] += currSize[cutAxis];
                currSize[cutAxis] = nextLine - lines[i];
                regions.Add(new Recti(currPos, currSize));
            }

            return regions.ToArray();
        }

        /// <summary>
        /// Determines if the given line segment overlaps one of the edges.
        /// </summary>
        public bool OverlapsEdge(Segment1i line, int otherAxis, bool isVertical)
        {
            if (isVertical)
                return (otherAxis == left || otherAxis == right - 1) && line.Overlaps(new Segment1i(yMin, yMax));
            else
                return (otherAxis == bottom || otherAxis == top - 1) && line.Overlaps(new Segment1i(xMin, xMax));
        }

        /// <summary>
        /// Determines if any one of the edges of the given rectangle overlaps one of the edges.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool OverlapsEdge(Recti other)
        {
            Segment1i h = new Segment1i(other.left, other.right),
                v = new Segment1i(other.bottom, other.top);
            return OverlapsEdge(h, other.bottom, false)
                || OverlapsEdge(v, other.left, true)
                || OverlapsEdge(h, other.top - 1, false)
                || OverlapsEdge(v, other.right - 1, true);
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", _pos, _size);
        }

        public override bool Equals(object obj)
        {
            return obj is Recti && this == (Recti)obj;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + position.GetHashCode();
            hash = hash * 23 + size.GetHashCode();
            return hash;
        }

        public IEnumerator<Vec2i> GetEnumerator()
        {
            // this order is efficient when looping through 2D arrays of T[x, y]
            // users usually don't care what the order is when using this enumerator.
            for (int x = left; x < right; x++)
            {
                for (int y = bottom; y < top; y++)
                {
                    yield return new Vec2i(x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
