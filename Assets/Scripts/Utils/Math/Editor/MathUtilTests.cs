using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace TX.Test
{
    [TestFixture]
    public class MathUtilTests
    {
        [Test]
        public void TestMod()
        {
            Assert.AreEqual(1, MathUtil.Mod(3, 2));
            Assert.AreEqual(0, MathUtil.Mod(42, 2));
            Assert.AreEqual(-1, -3 % 2);
            Assert.AreEqual(1, MathUtil.Mod(-3, 2));
            Assert.AreEqual(0, MathUtil.Mod(-42, 2));
        }

        [Test]
        public void TestGetDivisors()
        {
            CollectionAssert.AreEquivalent(new[] { 2, 4 }, MathUtil.GetDivisors(8));
            CollectionAssert.AreEquivalent(new[] { 2, 4, 7, 14 }, MathUtil.GetDivisors(28));
            CollectionAssert.AreEquivalent(new int[0], MathUtil.GetDivisors(7));
        }

        [Test]
        public void TestInRange()
        {
            Assert.IsFalse(MathUtil.InRange(1, -1, 0));
            Assert.IsFalse(MathUtil.InRange(1, 0, 0));

            Assert.IsTrue(MathUtil.InRange(1, 0, 2));
            Assert.IsTrue(MathUtil.InRange(1, 0, 1));   // inclusive
            Assert.IsTrue(MathUtil.InRange(1, 1, 2));   // inclusive

            Assert.IsFalse(MathUtil.InRange(1.5f, -1, 0));
            Assert.IsTrue(MathUtil.InRange(1.5f, 1, 2));
            Assert.IsFalse(MathUtil.InRange(1.5, 0, 1));
            Assert.IsTrue(MathUtil.InRange(1.5, 1.5, 2)); // inclusive
            Assert.IsTrue(MathUtil.InRange(1.5, 1, 1.5)); // inclusive
        }

        [Test]
        public void TestFloorToInt()
        {
            Assert.AreEqual(Vec2i.one, MathUtil.FloorToInt(new Vector2(1, 1)));
            Assert.AreEqual(Vec2i.one, MathUtil.FloorToInt(new Vector2(1.2f, 1.8f)));

            Assert.AreEqual(Vec2i.one * -1, MathUtil.FloorToInt(new Vector2(-1f, -1f)));
            Assert.AreEqual(Vec2i.one * -1, MathUtil.FloorToInt(new Vector2(-0.2f, -0.8f)));
        }

        [Test]
        public void TestToPercent()
        {
            Assert.AreEqual(5, MathUtil.ToPercent(0.05f));
        }
    }

    [TestFixture]
    public class Vec2iTests
    {
        //private Vec2i[] TestSet;

        //[TestFixtureSetUp]
        //public void SetUp()
        //{
        //    TestSet = new[]
        //    {
        //        new Vec2i(0,0),
        //        new Vec2i(1,1),
        //        new Vec2i(1,2),
        //        new Vec2i(10,20),
        //        new Vec2i(1000,2000),
        //        new Vec2i(-1, -1),
        //        new Vec2i(-1, -100),
        //        new Vec2i(-6000, -5),
        //        new Vec2i(0, -5),
        //        new Vec2i(387624, 0),
        //    };
        //}

        [Test]
        public void TestOperator()
        {
            Vec2i a = new Vec2i(1, 2);
            Vec2i b = new Vec2i(2, 3);
            Vec2i c = new Vec2i(2, 1);

            Assert.AreEqual(new Vec2i(4, 5), b + 2);
            Assert.AreEqual(new Vec2i(0, 1), b - 2);
            Assert.AreEqual(new Vec2i(4, 6), b * 2);
            Assert.AreEqual(new Vec2i(1, 1), b / 2);

            Assert.AreEqual(new Vec2i(3, 5), a + b);
            Assert.AreEqual(new Vec2i(-1, -1), a - b);
            Assert.AreEqual(new Vec2i(2, 6), a * b);
            Assert.AreEqual(new Vec2i(0, 0), a / b);

            Assert.IsTrue(a < b);
            Assert.IsFalse(a > b);
            Assert.IsTrue(b > a);
            Assert.IsFalse(b < a);
            Assert.IsFalse(a < new Vec2i(a));
            Assert.IsFalse(a > new Vec2i(a));
            Assert.IsFalse(a > c);
            Assert.IsFalse(a < c);

            Assert.IsTrue(a <= b);
            Assert.IsFalse(a >= b);
            Assert.IsTrue(b >= a);
            Assert.IsFalse(b <= a);
            Assert.IsTrue(a <= new Vec2i(a));
            Assert.IsTrue(a >= new Vec2i(a));
            Assert.IsFalse(a >= c);
            Assert.IsFalse(a <= c);

            Assert.IsTrue(a == new Vec2i(a));
            Assert.IsTrue(a != b);
        }

        [Test]
        public void TestMooreNeighbours()
        {
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new Vec2i(-1,-1),
                    new Vec2i(0,-1),
                    new Vec2i(-1, 0),
                    new Vec2i(-1, 1),
                    new Vec2i(1, -1),
                    new Vec2i(1, 0),
                    new Vec2i(0, 1),
                    new Vec2i(1, 1),
                },
                Vec2i.zero.MooreNeighbours());
        }

        [Test]
        public void TestVonNeumannNeighbours()
        {
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new Vec2i(-1, 0),
                    new Vec2i(0, -1),
                    new Vec2i(0, 1),
                    new Vec2i(1, 0),
                },
                Vec2i.zero.VonNeumannNeighbours());
        }

        [Test]
        public void TestMooreRing()
        {
            CollectionAssert.AreEquivalent(
                new[]
                {
                    // S
                    new Vec2i(0,0),
                    new Vec2i(1,0),
                    // E
                    new Vec2i(2,0),
                    new Vec2i(2,1),
                    // N
                    new Vec2i(2,2),
                    new Vec2i(1,2),
                    // W
                    new Vec2i(0,2),
                    new Vec2i(0,1),
                },
                Vec2i.one.MooreRing(1));

            CollectionAssert.AreEquivalent(
                new[]
                {
                    // S
                    new Vec2i(-2,-2),
                    new Vec2i(-1,-2),
                    new Vec2i(0,-2),
                    new Vec2i(1,-2),
                    // E
                    new Vec2i(2,-2),
                    new Vec2i(2,-1),
                    new Vec2i(2,0),
                    new Vec2i(2,1),
                    // N
                    new Vec2i(2,2),
                    new Vec2i(1,2),
                    new Vec2i(0,2),
                    new Vec2i(-1,2),
                    // W
                    new Vec2i(-2,2),
                    new Vec2i(-2,1),
                    new Vec2i(-2,0),
                    new Vec2i(-2,-1),
                },
                Vec2i.zero.MooreRing(2));
        }

        [Test]
        public void TestGo()
        {
            // Dir
            Assert.AreEqual(new Vec2i(0, 1), Vec2i.zero.Go(Dir.N));
            Assert.AreEqual(new Vec2i(-1, 0), Vec2i.zero.Go(Dir.W));
            Assert.AreEqual(new Vec2i(1, 0), Vec2i.zero.Go(Dir.E));
            Assert.AreEqual(new Vec2i(0, -1), Vec2i.zero.Go(Dir.S));

            Assert.AreEqual(new Vec2i(0, 5), Vec2i.zero.Go(Dir.N, 5));
            Assert.AreEqual(new Vec2i(-5, 0), Vec2i.zero.Go(Dir.W, 5));
            Assert.AreEqual(new Vec2i(5, 0), Vec2i.zero.Go(Dir.E, 5));
            Assert.AreEqual(new Vec2i(0, -5), Vec2i.zero.Go(Dir.S, 5));

            // Dir8
            Assert.AreEqual(new Vec2i(0, 1), Vec2i.zero.Go(Dir8.N));
            Assert.AreEqual(new Vec2i(1, 1), Vec2i.zero.Go(Dir8.NE));
            Assert.AreEqual(new Vec2i(1, 0), Vec2i.zero.Go(Dir8.E));
            Assert.AreEqual(new Vec2i(1, -1), Vec2i.zero.Go(Dir8.SE));
            Assert.AreEqual(new Vec2i(0, -1), Vec2i.zero.Go(Dir8.S));
            Assert.AreEqual(new Vec2i(-1, -1), Vec2i.zero.Go(Dir8.SW));
            Assert.AreEqual(new Vec2i(-1, 0), Vec2i.zero.Go(Dir8.W));

            Assert.AreEqual(new Vec2i(0, 7), Vec2i.zero.Go(Dir8.N, 7));
            Assert.AreEqual(new Vec2i(7, 7), Vec2i.zero.Go(Dir8.NE, 7));
            Assert.AreEqual(new Vec2i(7, 0), Vec2i.zero.Go(Dir8.E, 7));
            Assert.AreEqual(new Vec2i(7, -7), Vec2i.zero.Go(Dir8.SE, 7));
            Assert.AreEqual(new Vec2i(0, -7), Vec2i.zero.Go(Dir8.S, 7));
            Assert.AreEqual(new Vec2i(-7, -7), Vec2i.zero.Go(Dir8.SW, 7));
            Assert.AreEqual(new Vec2i(-7, 0), Vec2i.zero.Go(Dir8.W, 7));
        }
    }

    [TestFixture]
    public class Segment1iTests
    {
        [Test]
        public void TestInvalidRange()
        {
            new Segment1i(0, 1);
            new Segment1i(0, 0);

            Assert.Catch<ArgumentException>(() => new Segment1i(0, -1));
        }

        [Test]
        public void TestOverlaps()
        {
            Assert.IsFalse(new Segment1i(0, 2).Overlaps(new Segment1i(2, 3)));
            Assert.IsTrue(new Segment1i(0, 2).Overlaps(new Segment1i(1, 3)));
        }

        [Test]
        public void TestDistance()
        {
            Assert.AreEqual(3, new Segment1i(0, 2).Distance(new Segment1i(4, 5)));
        }

        [Test]
        public void TestIntersect()
        {
            Assert.AreEqual(new Segment1i(1, 2), new Segment1i(0, 2).Intersect(new Segment1i(1, 3)));
            Assert.AreEqual(new Segment1i(0, 2), new Segment1i(0, 2).Intersect(new Segment1i(0, 3)));
            Assert.AreEqual(new Segment1i(0, 2), new Segment1i(0, 2).Intersect(new Segment1i(-1, 3)));

            Assert.AreEqual(0, new Segment1i(2, 2).Intersect(new Segment1i(0, 5)).length);
        }

        [Test]
        public void TestUnion()
        {
            Assert.AreEqual(new Segment1i(0, 3), new Segment1i(0, 2).Union(new Segment1i(1, 3)));
            Assert.AreEqual(new Segment1i(0, 3), new Segment1i(0, 2).Union(new Segment1i(0, 3)));
            Assert.AreEqual(new Segment1i(-1, 3), new Segment1i(0, 2).Union(new Segment1i(-1, 3)));

            Assert.AreEqual(3, new Segment1i(0, 0).Union(new Segment1i(3, 3)).length);
            Assert.AreEqual(new Segment1i(0, 1), new Segment1i(0, 0).Union(new Segment1i(0, 1)));
        }

        [Test]
        public void TestLerp()
        {
            var origin = new Segment1i(0, 5);

            CollectionAssert.AreEqual(new int[0], origin.Lerp(origin));

            var expansion = new Segment1i(-2, 7);
            var expansion_1 = new Segment1i(-3, 6);
            CollectionAssert.AreEqual(new[] { new Segment1i(-1, 6) }, origin.Lerp(expansion));
            CollectionAssert.AreEqual(
                new[] {
                    new Segment1i(-1, 6),
                    new Segment1i(-2, 6),
                }, origin.Lerp(expansion_1));

            var contraction = new Segment1i(2, 3);
            var contraction_1 = new Segment1i(3, 5);
            CollectionAssert.AreEqual(new[] { new Segment1i(1, 4) }, origin.Lerp(contraction));
            CollectionAssert.AreEqual(
                new[]
                {
                    new Segment1i(1,5),
                    new Segment1i(2,5),
                }, origin.Lerp(contraction_1));

            var shifting = new Segment1i(4, 9);
            var shifting_1 = new Segment1i(7, 10);
            CollectionAssert.AreEqual(
                new[] {
                    new Segment1i(1, 6),
                    new Segment1i(2, 7),
                    new Segment1i(3, 8),
                }, origin.Lerp(shifting));
            CollectionAssert.AreEqual(
                new[] {
                    new Segment1i(1, 6),
                    new Segment1i(2, 7),
                    new Segment1i(3, 8),
                    new Segment1i(4, 9),
                    new Segment1i(5, 10),
                    new Segment1i(6, 10),
                }, origin.Lerp(shifting_1));
        }

        [Test]
        public void TestEnumerator()
        {
            CollectionAssert.AreEqual(new int[0], new Segment1i(0, 0));
            CollectionAssert.AreEqual(new int[0], new Segment1i(5, 5));

            CollectionAssert.AreEqual(new[] { 3 }, new Segment1i(3, 4));

            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4, 5 }, new Segment1i(0, 6));
            CollectionAssert.AreEqual(new[] { 5, 6, 7, 8, 9 }, new Segment1i(5, 10));
        }
    }

    [TestFixture]
    public class Segment2iTests
    {
        [Test]
        public void TestGetCenter()
        {
            Assert.AreEqual(Vec2i.zero, new Segment2i(new Vec2i(-1, 0), new Vec2i(1, 0)).center);
            Assert.AreEqual(Vec2i.zero, new Segment2i(new Vec2i(-1, 2), new Vec2i(1, -2)).center);
            Assert.AreEqual(Vec2i.one, new Segment2i(new Vec2i(0, 3), new Vec2i(2, -1)).center);
        }
    }

    [TestFixture]
    public class RectiSideTests
    {
        [Test]
        public void TestCells()
        {
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new Vec2i(3, 4),
                    new Vec2i(3, 5),
                    new Vec2i(3, 6),
                },
                new RectiSide(Dir.E, 4, 7, 3).cells.ToArray());

            CollectionAssert.AreEquivalent(
                new[]
                {
                    new Vec2i(4, -1),
                    new Vec2i(5, -1),
                    new Vec2i(6, -1),
                    new Vec2i(7, -1),
                },
                new RectiSide(Dir.S, 4, 8, -1).cells.ToArray());
        }
    }

    [TestFixture]
    public class RectiTests
    {
        private Tuple<Recti, Recti>[] StrictOverlapSet = new[]
        {
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(3, 3, 5, 5)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(0, 0, 4, 6)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(-1, -1, 5, 5)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(-1, 1, 5, 5)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(1, -1, 5, 5)),
            Tuple.New(new Recti(-3, -1, 7, 4), new Recti(0, 0, 9, 9)),
        };

        private Tuple<Recti, Recti>[] ContainSet = new[]
        {
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(1, 1, 3, 3)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(0, 0, 5, 5)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(0, 0, 4, 4)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(1, 2, 2, 1)),
            Tuple.New(new Recti(-5, -5, 2, 2), new Recti(-5, -5, 2, 1)),
            Tuple.New(new Recti(-5, -5, 2, 2), new Recti(-5, -4, 1, 1)),
            Tuple.New(new Recti(-5, -5, 10, 10), new Recti(-1, -5, 1, 1)),
        };

        private Tuple<Recti, Recti>[] OverlapEdgeSet = new[]
        {
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(0, 0, 1, 1)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(0, 3, 7, 1)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(-3, -3, 4, 4)),
        };

        //private Tuple<Recti, Recti>[] AdjacentEdgeSet = new[]
        //{
        //    Tuple.New(new Recti(0, 0, 5, 5), new Recti(-1, -1, 2, 1)),
        //    Tuple.New(new Recti(0, 0, 5, 5), new Recti(-1, -1, 10, 1)),
        //    Tuple.New(new Recti(0, 0, 5, 5), new Recti(-1, -1, 1, 2)),
        //    Tuple.New(new Recti(0, 0, 5, 5), new Recti(-1, -1, 1, 10)),
        //    Tuple.New(new Recti(-5, -5, 5, 5), new Recti(-5, 0, 1, 1)),
        //    Tuple.New(new Recti(-5, -5, 5, 5), new Recti(0, -5, 1, 1)),
        //    Tuple.New(new Recti(-5, 5, 5, 5), new Recti(0, -1, 1, 1)),
        //    Tuple.New(new Recti(-5, 5, 5, 5), new Recti(-1, 0, 1, 1)),
        //};

        private Tuple<Recti, Recti>[] NonAdjacentSet = new[]
        {
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(-1, -1, 1, 1)),
            Tuple.New(new Recti(0, 0, 5, 5), new Recti(-2, 0, 1, 5)),
        };

        private Recti[] TestSet = new[]
        {
            new Recti(0, 0, 1, 1),
            new Recti(-1, -1, 1, 1),
            new Recti(-2, 2, 4, 4),
            new Recti(0, 0, 5, 6),
            new Recti(-10, 3, 7, 2),
            new Recti(2, 0, 1, 200),
        };

        [Test]
        public void TestMinMaxRecti()
        {
            Assert.AreEqual(new Recti(1, 2, 2, 2), Recti.MinMaxRecti(1, 2, 3, 4));
            Assert.AreEqual(new Recti(-1, -2, 3, 4), Recti.MinMaxRecti(-1, -2, 2, 2));
        }

        [Test]
        public void TestGetCornerAfter()
        {
            foreach (Recti r in TestSet)
            {
                Assert.AreEqual(r.bottomRight, r.GetCornerAfter(Dir.S));
                Assert.AreEqual(r.topRight, r.GetCornerAfter(Dir.E));
                Assert.AreEqual(r.topLeft, r.GetCornerAfter(Dir.N));
                Assert.AreEqual(r.bottomLeft, r.GetCornerAfter(Dir.W));
            }
        }

        [Test]
        public void TestGetSideOn()
        {
            Recti rect = new Recti(0, 0, 4, 8);

            Assert.AreEqual(
                new RectiSide(Dir.E, 0, 8, 3),
                rect.GetSideOn(Dir.E));
            Assert.AreEqual(
                new RectiSide(Dir.E, 0, 7, 3),
                rect.GetSideOn(Dir.E, true));

            Assert.AreEqual(
                new RectiSide(Dir.N, 0, 4, 7),
                rect.GetSideOn(Dir.N));
            Assert.AreEqual(
                new RectiSide(Dir.N, 1, 4, 7),
                rect.GetSideOn(Dir.N, true));

            Assert.AreEqual(
                new RectiSide(Dir.S, 0, 4, 0),
                rect.GetSideOn(Dir.S));
            Assert.AreEqual(
                new RectiSide(Dir.S, 0, 3, 0),
                rect.GetSideOn(Dir.S, true));

            Assert.AreEqual(
                new RectiSide(Dir.W, 0, 8, 0),
                rect.GetSideOn(Dir.W));
            Assert.AreEqual(
                new RectiSide(Dir.W, 1, 8, 0),
                rect.GetSideOn(Dir.W, true));
        }

        [Test]
        public void TestContains()
        {
            // Vec2i
            Assert.IsFalse(new Recti(0, 0, 1, 1).Contains(Vec2i.one));
            Assert.IsTrue(new Recti(0, 0, 1, 1).Contains(Vec2i.zero));

            Assert.IsFalse(new Recti(-1, -4, 2, 8).Contains(new Vec2i(10, 10)));
            Assert.IsTrue(new Recti(-1, -4, 2, 8).Contains(new Vec2i(-1, -4)));
            Assert.IsTrue(new Recti(-1, -4, 2, 8).Contains(Vec2i.zero));
            Assert.IsTrue(new Recti(-1, -4, 2, 8).Contains(new Vec2i(0, 3)));
            Assert.IsFalse(new Recti(-1, -4, 2, 8).Contains(new Vec2i(1, 3)));
            Assert.IsFalse(new Recti(-1, -4, 2, 8).Contains(new Vec2i(0, 4)));

            // Recti
            TestSet.ForEach((i, rect) =>
            {
                Assert.IsTrue(rect.Contains(rect), "[{0}] R={1}", i, rect);
            });
            ContainSet.ForEach((i, test) =>
            {
                Assert.IsTrue(test.Item1.Contains(test.Item2), "[{0}] R1={1}, R2={2}", i, test.Item1, test.Item2);
            });
            StrictOverlapSet.ForEach((i, test) =>
            {
                Assert.IsFalse(test.Item1.Contains(test.Item2), "[{0}] R1={1}, R2={2}", i, test.Item1, test.Item2);
            });
            NonAdjacentSet.ForEach((i, test) =>
            {
                Assert.IsFalse(test.Item1.Contains(test.Item2), "[{0}] R1={1}, R2={2}", i, test.Item1, test.Item2);
            });
        }

        [Test]
        public void TestOverlaps()
        {
            TestSet.ForEach((i, rect) =>
            {
                Assert.IsTrue(rect.Overlaps(rect), "[{0}] R={1}", i, rect);
            });

            ContainSet.ForEach((i, test) =>
            {
                Assert.IsTrue(test.Item1.Overlaps(test.Item2), "[{0}] R1={1}, R2={2}", i, test.Item1, test.Item2);
                Assert.IsTrue(test.Item2.Overlaps(test.Item1), "[{0}] R1={2}, R2={1}", i, test.Item1, test.Item2);
            });

            StrictOverlapSet.ForEach((i, test) =>
            {
                Assert.IsTrue(test.Item1.Overlaps(test.Item2), "[{0}] R1={1}, R2={2}", i, test.Item1, test.Item2);
                Assert.IsTrue(test.Item2.Overlaps(test.Item1), "[{0}] R1={2}, R2={1}", i, test.Item1, test.Item2);
            });

            NonAdjacentSet.ForEach((i, test) =>
            {
                Assert.IsFalse(test.Item1.Overlaps(test.Item2), "[{0}] R1={1}, R2={2}", i, test.Item1, test.Item2);
                Assert.IsFalse(test.Item2.Overlaps(test.Item1), "[{0}] R1={2}, R2={1}", i, test.Item1, test.Item2);
            });
        }

        [Test]
        public void TestMove()
        {
            Assert.AreEqual(new Recti(1, 2, 3, 4), new Recti(0, 0, 3, 4).Move(new Vec2i(1, 2)));
            Assert.AreEqual(new Recti(-1, -2, 5, 5), new Recti(0, 0, 5, 5).Move(new Vec2i(-1, -2)));
            Assert.AreEqual(new Recti(1, 2, 3, 4), new Recti(1, 2, 3, 4).Move(Vec2i.zero));
        }

        [Test]
        public void TestMoveTo()
        {
            TestSet.ForEach((i, rect) =>
            {
                Assert.AreEqual(
                    new Recti(new Vec2i(3, 6), rect.size),
                    rect.MoveTo(new Vec2i(3, 6)),
                    "[{0}] R={1}", i, rect);
            });
        }

        [Test]
        public void TestExpand()
        {
            Assert.AreEqual(new Recti(0, 0, 5, 5), new Recti(1, 1, 3, 3).Expand(Vec2i.one));
            Assert.AreEqual(new Recti(1, 1, 3, 3), new Recti(1, 1, 3, 3).Expand(Vec2i.zero));
            Assert.AreEqual(new Recti(0, -1, 5, 7), new Recti(1, 1, 3, 3).Expand(new Vec2i(1, 2)));

            Assert.AreEqual(new Recti(-1, 0, 4, 3), new Recti(0, 0, 3, 3).Expand(Dir.W, 1));
        }

        [Test]
        public void TestExpandToInclude()
        {
            Recti r = new Recti(0, 0, 0, 0);

            r.ExpandToInclude(new Vec2i(1, 1));
            Assert.AreEqual(new Recti(1, 1, 1, 1), r);

            r.ExpandToInclude(new Vec2i(1, 1));
            Assert.AreEqual(new Recti(1, 1, 1, 1), r);

            r.ExpandToInclude(new Vec2i(3, 1));
            Assert.AreEqual(new Recti(1, 1, 3, 1), r);

            r.ExpandToInclude(new Vec2i(0, 4));
            Assert.AreEqual(new Recti(0, 1, 4, 4), r);
        }

        [Test]
        public void TestShrink()
        {
            Assert.AreEqual(new Recti(1, 1, 3, 3), new Recti(0, 0, 5, 5).Shrink(Vec2i.one));
            Assert.AreEqual(new Recti(1, 1, 3, 3), new Recti(1, 1, 3, 3).Shrink(Vec2i.zero));
            Assert.AreEqual(new Recti(1, 1, 3, 3), new Recti(0, -1, 5, 7).Shrink(new Vec2i(1, 2)));

            Assert.AreEqual(new Recti(0, 0, 3, 3), new Recti(-1, 0, 4, 3).Shrink(Dir.W, 1));
        }

        [Test]
        public void TestDivide()
        {
            CollectionAssert.AreEqual(
                new[]
                {
                    new Recti(0, 0, 2, 4),
                    new Recti(2, 0, 2, 4),
                },
                new Recti(0, 0, 4, 4).Divide(true, 2));

            CollectionAssert.AreEqual(
                new[]
                {
                    new Recti(0, 0, 4, 1),
                    new Recti(0, 1, 4, 3),
                },
                new Recti(0, 0, 4, 4).Divide(false, 1));
        }

        [Test]
        public void TestMultiCut()
        {
            CollectionAssert.AreEqual(
                new[]
                {
                    new Recti(0, 0, 2, 4),
                    new Recti(2, 0, 2, 4),
                    new Recti(4, 0, 1, 4),
                    new Recti(5, 0, 3, 4),
                },
                new Recti(0, 0, 8, 4).MultiCut(true, new[] { 2, 4, 5 }));

            CollectionAssert.AreEqual(
                new[]
                {
                    new Recti(0, 0, 4, 1),
                    new Recti(0, 1, 4, 5),
                    new Recti(0, 6, 4, 1),
                },
                new Recti(0, 0, 4, 7).MultiCut(false, new[] { 1, 6 }));
        }

        [Test]
        public void TestOverlapsEdge()
        {
            OverlapEdgeSet.ForEach((i, test) =>
            {
                Assert.IsTrue(test.Item1.OverlapsEdge(test.Item1), "[{0}] R={1}", i, test.Item1);
                Assert.IsTrue(test.Item1.OverlapsEdge(test.Item2), "[{0}] R1={1}, R2={2}", i, test.Item1, test.Item2);
                Assert.IsTrue(test.Item2.OverlapsEdge(test.Item1), "[{0}] R1={2}, R2={1}", i, test.Item1, test.Item2);
            });

            NonAdjacentSet.ForEach((i, test) =>
            {
                Assert.IsFalse(test.Item1.OverlapsEdge(test.Item2), "[{0}] R1={1}, R2={2}", i, test.Item1, test.Item2);
            });
        }
    }
}
