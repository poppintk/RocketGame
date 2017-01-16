using System;
using UnityEngine;

namespace TX
{
    /// <summary>
    /// Enum of 4 directions.
    /// Use this alphabet order since they are frequently used in names.
    /// </summary>
    public enum Dir { E, N, S, W }

    [Flags]
    public enum DirFlags
    {
        NONE = 0,
        E = 1 << 0,
        N = 1 << 1,
        W = 1 << 2,
        S = 1 << 3,
        ALL = E | N | W | S,
    }

    /// <summary> Enum of 8 directions, in CCW order. </summary>
    public enum Dir8 { E, NE, N, NW, W, SW, S, SE }

    public static class DirectionExt
    {
        #region Dir

        private static readonly Vec2i[] Vec2iMap = new Vec2i[4]
        {
        Vec2i.right,
        Vec2i.up,
        Vec2i.down,
        Vec2i.left,
        };

        private static readonly int[] SignMap = new[] { 1, 1, -1, -1 };
        private static readonly Dir[] CWMap = new[] { Dir.S, Dir.E, Dir.W, Dir.N };
        private static readonly Dir8[] Dir8Map = new[] { Dir8.E, Dir8.N, Dir8.S, Dir8.W };
        private static readonly DirFlags[] FlagMap = new[] { DirFlags.E, DirFlags.N, DirFlags.S, DirFlags.W };

        public static Dir Opposite(this Dir dir)
        {
            return (Dir)(3 - (int)dir);
        }

        public static int Sign(this Dir dir)
        {
            return SignMap[(int)dir];
        }

        public static bool IsVertical(this Dir dir)
        {
            return dir == Dir.S || dir == Dir.N;
        }

        public static int Axis(this Dir dir)
        {
            return Convert.ToInt32(dir.IsVertical());
        }

        public static Vec2i ToVec2i(this Dir dir)
        {
            return Vec2iMap[(int)dir];
        }

        public static DirFlags ToFlag(this Dir dir)
        {
            return FlagMap[(int)dir];
        }

        public static Dir8 ToDir8(this Dir dir)
        {
            return Dir8Map[(int)dir];
        }

        public static Dir NextCW(this Dir dir)
        {
            return CWMap[(int)dir];
        }

        public static Dir NextCCW(this Dir dir)
        {
            return dir.NextCW().Opposite();
        }

        #endregion

        #region Dir8

        private static readonly Vec2i[] Vec2iMap8 = new Vec2i[]
        {
            Vec2i.right,
            new Vec2i(1,1),
            Vec2i.up,
            new Vec2i(-1,1),
            Vec2i.left,
            new Vec2i(-1,-1),
            Vec2i.down,
            new Vec2i(1,-1),
        };

        public static Vec2i ToVec2i(this Dir8 dir)
        {
            return Vec2iMap8[(int)dir];
        }

        public static Dir8 NextCW(this Dir8 dir)
        {
            return (Dir8)(((int)dir + 7) % 8);
        }

        public static Dir8 NextCCW(this Dir8 dir)
        {
            return (Dir8)(((int)dir + 1) % 8);
        }

        public static Dir8 Opposite(this Dir8 dir)
        {
            return (Dir8)(((int)dir + 4) % 8);
        }

        #endregion

        #region DirFlag

        public static DirFlags Rotate(this DirFlags flag, Rot4 rot = Rot4.CCW90)
        {
            int f = (int)flag, r = (int)rot;
            int shifted = f << r;
            return (DirFlags)((shifted & 0xF) | ((shifted & 0xF0) >> 4));
        }

        /// <summary>Convert flag into a direction, if multiple flags are set, return the first set direction.</summary>
        /// <param name="flag">The flag.</param>
        /// <exception cref="System.ArgumentException">Flag has no bit set.</exception>
        public static Dir ToDir(this DirFlags flag)
        {
            foreach (Dir d in CWMap)
                if (flag.HasFlag(FlagMap[(int)d]))
                    return d;
            throw new ArgumentException("Flag has no bit set.");
        }

        #endregion
    }
}
