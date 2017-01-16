using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TX.Test
{
    public class DirTests
    {
        [Test]
        public void TestOpposite()
        {
            Assert.AreEqual(Dir.W, Dir.E.Opposite());
            Assert.AreEqual(Dir.E, Dir.W.Opposite());
            Assert.AreEqual(Dir.S, Dir.N.Opposite());
            Assert.AreEqual(Dir.N, Dir.S.Opposite());
        }

        [Test]
        public void TestSign()
        {
            Assert.AreEqual(-1, Dir.W.Sign());
            Assert.AreEqual(-1, Dir.S.Sign());
            Assert.AreEqual(1, Dir.N.Sign());
            Assert.AreEqual(1, Dir.E.Sign());
        }

        [Test]
        public void TestIsVertical()
        {
            Assert.IsFalse(Dir.W.IsVertical());
            Assert.IsFalse(Dir.E.IsVertical());
            Assert.IsTrue(Dir.N.IsVertical());
            Assert.IsTrue(Dir.S.IsVertical());
        }

        [Test]
        public void TestAxis()
        {
            Assert.AreEqual(0, Dir.W.Axis());
            Assert.AreEqual(0, Dir.E.Axis());
            Assert.AreEqual(1, Dir.N.Axis());
            Assert.AreEqual(1, Dir.S.Axis());
        }

        [Test]
        public void TestToVec2i()
        {
            Assert.AreEqual(Vec2i.left, Dir.W.ToVec2i());
            Assert.AreEqual(Vec2i.right, Dir.E.ToVec2i());
            Assert.AreEqual(Vec2i.up, Dir.N.ToVec2i());
            Assert.AreEqual(Vec2i.down, Dir.S.ToVec2i());
        }

        [Test]
        public void TestToFlag()
        {
            Assert.AreEqual(DirFlags.W, Dir.W.ToFlag());
            Assert.AreEqual(DirFlags.E, Dir.E.ToFlag());
            Assert.AreEqual(DirFlags.N, Dir.N.ToFlag());
            Assert.AreEqual(DirFlags.S, Dir.S.ToFlag());
        }

        [Test]
        public void TestNextCW()
        {
            Assert.AreEqual(Dir.N, Dir.W.NextCW());
            Assert.AreEqual(Dir.E, Dir.N.NextCW());
            Assert.AreEqual(Dir.S, Dir.E.NextCW());
            Assert.AreEqual(Dir.W, Dir.S.NextCW());
        }

        [Test]
        public void TestNextCCW()
        {
            Assert.AreEqual(Dir.S, Dir.W.NextCCW());
            Assert.AreEqual(Dir.E, Dir.S.NextCCW());
            Assert.AreEqual(Dir.N, Dir.E.NextCCW());
            Assert.AreEqual(Dir.W, Dir.N.NextCCW());
        }
    }

    public class Dir8Tests
    {
        [Test]
        public void TestToVec2i()
        {
            Assert.AreEqual(Vec2i.right, Dir8.E.ToVec2i());
            Assert.AreEqual(new Vec2i(1, 1), Dir8.NE.ToVec2i());
            Assert.AreEqual(Vec2i.up, Dir8.N.ToVec2i());
            Assert.AreEqual(new Vec2i(-1, 1), Dir8.NW.ToVec2i());
            Assert.AreEqual(Vec2i.left, Dir8.W.ToVec2i());
            Assert.AreEqual(new Vec2i(-1, -1), Dir8.SW.ToVec2i());
            Assert.AreEqual(Vec2i.down, Dir8.S.ToVec2i());
            Assert.AreEqual(new Vec2i(1, -1), Dir8.SE.ToVec2i());
        }

        [Test]
        public void TestNextCW()
        {
            Assert.AreEqual(Dir8.SE, Dir8.E.NextCW());
            Assert.AreEqual(Dir8.S, Dir8.SE.NextCW());
            Assert.AreEqual(Dir8.SW, Dir8.S.NextCW());
            Assert.AreEqual(Dir8.W, Dir8.SW.NextCW());
            Assert.AreEqual(Dir8.NW, Dir8.W.NextCW());
            Assert.AreEqual(Dir8.N, Dir8.NW.NextCW());
            Assert.AreEqual(Dir8.NE, Dir8.N.NextCW());
            Assert.AreEqual(Dir8.E, Dir8.NE.NextCW());
        }

        [Test]
        public void TestNextCCW()
        {
            Assert.AreEqual(Dir8.NE, Dir8.E.NextCCW());
            Assert.AreEqual(Dir8.N, Dir8.NE.NextCCW());
            Assert.AreEqual(Dir8.NW, Dir8.N.NextCCW());
            Assert.AreEqual(Dir8.W, Dir8.NW.NextCCW());
            Assert.AreEqual(Dir8.SW, Dir8.W.NextCCW());
            Assert.AreEqual(Dir8.S, Dir8.SW.NextCCW());
            Assert.AreEqual(Dir8.SE, Dir8.S.NextCCW());
            Assert.AreEqual(Dir8.E, Dir8.SE.NextCCW());
        }

        [Test]
        public void TestOpposite()
        {
            Assert.AreEqual(Dir8.W, Dir8.E.Opposite());
            Assert.AreEqual(Dir8.NW, Dir8.SE.Opposite());
            Assert.AreEqual(Dir8.N, Dir8.S.Opposite());
            Assert.AreEqual(Dir8.NE, Dir8.SW.Opposite());
            Assert.AreEqual(Dir8.E, Dir8.W.Opposite());
            Assert.AreEqual(Dir8.SE, Dir8.NW.Opposite());
            Assert.AreEqual(Dir8.S, Dir8.N.Opposite());
            Assert.AreEqual(Dir8.SW, Dir8.NE.Opposite());
        }
    }

    public class DirFlagsTest
    {
        [Test]
        public void TestRotate()

        {
            foreach (var rot in EnumUtil.GetValues<Rot4>())
            {
                Assert.AreEqual(DirFlags.ALL, DirFlags.ALL.Rotate(rot));
                Assert.AreEqual(DirFlags.NONE, DirFlags.NONE.Rotate(rot));
            }

            foreach (var flags in EnumUtil.GetValues<DirFlags>())
            {
                Assert.AreEqual(flags, flags.Rotate(Rot4.None));
            }

            Assert.AreEqual(DirFlags.S, DirFlags.E.Rotate(Rot4.CW90));
            Assert.AreEqual(DirFlags.W, DirFlags.S.Rotate(Rot4.CW90));
            Assert.AreEqual(DirFlags.N, DirFlags.W.Rotate(Rot4.CW90));
            Assert.AreEqual(DirFlags.E, DirFlags.N.Rotate(Rot4.CW90));

            Assert.AreEqual(DirFlags.N, DirFlags.E.Rotate(Rot4.CCW90));
            Assert.AreEqual(DirFlags.W, DirFlags.N.Rotate(Rot4.CCW90));
            Assert.AreEqual(DirFlags.S, DirFlags.W.Rotate(Rot4.CCW90));
            Assert.AreEqual(DirFlags.E, DirFlags.S.Rotate(Rot4.CCW90));

            Assert.AreEqual(DirFlags.W, DirFlags.E.Rotate(Rot4.Rev));
            Assert.AreEqual(DirFlags.N, DirFlags.S.Rotate(Rot4.Rev));
            Assert.AreEqual(DirFlags.S, DirFlags.N.Rotate(Rot4.Rev));
            Assert.AreEqual(DirFlags.E, DirFlags.W.Rotate(Rot4.Rev));

            Assert.AreEqual(
                DirFlags.W | DirFlags.E,
                (DirFlags.N | DirFlags.S).Rotate(Rot4.CW90));

            Assert.AreEqual(
                DirFlags.W | DirFlags.S,
                (DirFlags.N | DirFlags.W).Rotate(Rot4.CCW90));

            Assert.AreEqual(
                DirFlags.W | DirFlags.S | DirFlags.E,
                (DirFlags.W | DirFlags.N | DirFlags.E).Rotate(Rot4.Rev));
        }

        [Test]
        public void TestRotate2()
        {
            var testCases = new[]
            {
                new[] { DirFlags.ALL, DirFlags.ALL, DirFlags.ALL, DirFlags.ALL },
                new[] { DirFlags.NONE, DirFlags.NONE, DirFlags.NONE, DirFlags.NONE },
                new[] { DirFlags.E, DirFlags.N, DirFlags.W, DirFlags.S },
                new[] { DirFlags.E | DirFlags.N, DirFlags.N | DirFlags.W, DirFlags.W | DirFlags.S, DirFlags.S | DirFlags.E },
                new[] { DirFlags.E | DirFlags.W, DirFlags.N | DirFlags.S, DirFlags.E | DirFlags.W, DirFlags.N | DirFlags.S },
            };
            foreach (var testCase in testCases)
            {
                for (int i = 0; i < 4; i++)
                {
                    var original = testCase[i];
                    var expected = testCase[(i + 1) % 4];
                    var actual = original.Rotate(Rot4.CCW90);
                    Assert.AreEqual(expected, actual, string.Format("{0} + CCW90", original));
                }
            }
        }

        [Test]
        public void TestToDir()
        {
            Assert.Catch<ArgumentException>(() => DirFlags.NONE.ToDir());

            Assert.AreEqual(Dir.E, DirFlags.E.ToDir());
            Assert.AreEqual(Dir.N, DirFlags.N.ToDir());
            Assert.AreEqual(Dir.W, DirFlags.W.ToDir());
            Assert.AreEqual(Dir.S, DirFlags.S.ToDir());

            Assert.Contains((DirFlags.E | DirFlags.N).ToDir(), new[] { Dir.E, Dir.N });
            Assert.Contains((DirFlags.N | DirFlags.W).ToDir(), new[] { Dir.N, Dir.W });
            Assert.Contains((DirFlags.W | DirFlags.S).ToDir(), new[] { Dir.W, Dir.S });
            Assert.Contains((DirFlags.S | DirFlags.E).ToDir(), new[] { Dir.S, Dir.E });
        }
    }
}
