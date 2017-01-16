using System;
using NUnit.Framework;
using UnityEngine;

namespace TX.Test
{
    [TestFixture]
    public class UnityExtensionsTests
    {
        #region Rect
        private Rect SampleRect = new Rect(-2, 1.1f, 3.5f, 9.7f);

        [Test]
        public void TestArea()
        {
            Assert.AreEqual(33.95, SampleRect.Area(), 1e-5f);
        }

        [Test]
        public void TestBottomLeft()
        {
            Assert.AreEqual(new Vector2(-2, 1.1f), SampleRect.BottomLeft());
        }

        [Test]
        public void TestTopLeft()
        {
            Assert.AreEqual(new Vector2(-2, 10.8f), SampleRect.TopLeft());
        }

        [Test]
        public void TestTopRight()
        {
            Assert.AreEqual(new Vector2(1.5f, 10.8f), SampleRect.TopRight());
        }

        [Test]
        public void TestBottomRight()
        {
            Assert.AreEqual(new Vector2(1.5f, 1.1f), SampleRect.BottomRight());
        }

        [Test]
        public void TestMult()
        {
            Assert.AreEqual(new Rect(-20, 11, 35, 97), SampleRect.Mult(10));
        }

        [Test]
        public void TestShrink()
        {
            Assert.AreEqual(new Rect(-1.5f, 1.6f, 2.5f, 8.7f), SampleRect.Shrink(0.5f));
            Assert.AreEqual(new Rect(-1, 1.6f, 1.5f, 8.7f), SampleRect.Shrink(1, 0.5f));
        }

        [Test]
        public void TestExpand()
        {
            Assert.AreEqual(new Rect(-2.5f, 0.6f, 4.5f, 10.7f), SampleRect.Expand(0.5f));
            Assert.AreEqual(new Rect(-3, 0.6f, 5.5f, 10.7f), SampleRect.Expand(1, 0.5f));
        }

        [Test]
        public void TestGetCorners()
        {
            CollectionAssert.AreEquivalent(
                new[]
                {
                    SampleRect.TopLeft(),
                    SampleRect.TopRight(),
                    SampleRect.BottomLeft(),
                    SampleRect.BottomRight(),
                },
                SampleRect.GetCorners());
        }

        [Test]
        public void TestAspectRatio()
        {
            Assert.AreEqual(SampleRect.width / SampleRect.height, SampleRect.AspectRatio());
        }

        [Test]
        public void TestScale()
        {
            //Vector2 pos = SampleRect.position, size = SampleRect.size;

            //// pivot = bottom left
            //Assert.AreEqual(
            //    new Rect(pos, size * 10),
            //    SampleRect.Scale(10f, SampleRect.BottomLeft()));

            //// pivot = top right
            //Assert.AreEqual(
            //    new Rect(pos - size * 10, size * 10),
            //    SampleRect.Scale(10f, SampleRect.TopRight()));

            //// pivot = top left
            //Assert.AreEqual(
            //    new Rect(pos.x, pos.y - size.y * 5, size.x * 10, size.y * 10),
            //    SampleRect.Scale(10f, SampleRect.TopLeft()));

            //// pivot = bottom right
            //Assert.AreEqual(
            //    new Rect(pos.x - size.x * 5, pos.y, size.x * 10, size.y * 10),
            //    SampleRect.Scale(10f, SampleRect.BottomRight()));

            //// pivot = center
            //Assert.AreEqual(
            //    new Rect(pos - size * 5, size * 10),
            //    SampleRect.Scale(10f, SampleRect.center));

            //// scale x y separately
            //Assert.AreEqual(
            //    new Rect(
            //        pos.x - size.x * 2.5f,
            //        pos.y - size.y * 5f,
            //        size.x * 5f,
            //        size.y * 10f),
            //    SampleRect.Scale(new Vector2(5, 10), SampleRect.center));
        }

        [Test]
        public void TestMove()
        {
            Vector2 offset = new Vector2(1, 2);
            Assert.AreEqual(
                new Rect(SampleRect.position + offset, SampleRect.size),
                SampleRect.Move(offset));
        }

        #endregion

        #region Ray

        [Test]
        public void TestIntersect()
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Vector3 point;

            Assert.IsFalse(new Ray(Vector3.one, Vector3.one).Intersect(plane, out point));
            Assert.IsFalse(new Ray(Vector3.one, Vector3.right).Intersect(plane, out point));// parallel to the plane

            Assert.IsTrue(new Ray(-Vector3.one, Vector3.one).Intersect(plane, out point)); // from opposite side
            Assert.AreEqual(Vector3.zero, point);
            Assert.IsTrue(new Ray(Vector3.one, Vector3.down).Intersect(plane, out point));
            Assert.AreEqual(new Vector3(1, 0, 1), point);
            Assert.IsTrue(new Ray(Vector3.one, -Vector3.one).Intersect(plane, out point));
            Assert.AreEqual(Vector3.zero, point);
        }

        #endregion

        #region Transform
        // TODO
        #endregion

        #region Vector

        [Test]
        public void TestToXZ()
        {
            Vector2 origin = new Vector2(1, 2);
            Assert.AreEqual(new Vector3(1, 0, 2), origin.ToXZ());
            Assert.AreEqual(new Vector3(1, 100, 2), origin.ToXZ(100));
        }

        [Test]
        public void TestGetXZ()
        {
            Assert.AreEqual(new Vector2(1, 3), new Vector3(1, 2, 3).GetXZ());
        }

        #endregion

        #region Camera
        // skipped
        #endregion

        #region Color

        [Test]
        public void TestToHex()
        {
            Assert.AreEqual("000000FF", new Color32(0, 0, 0, 255).ToHex());
            Assert.AreEqual("FFFFFF70", new Color32(255, 255, 255, 112).ToHex());
            Assert.AreEqual("400000FF", new Color32(64, 0, 0, 255).ToHex());
            Assert.AreEqual("007000FF", new Color32(0, 112, 0, 255).ToHex());
            Assert.AreEqual("0000AAFF", new Color32(0, 0, 170, 255).ToHex());
            Assert.AreEqual("FFFFFF00", new Color32(255, 255, 255, 0).ToHex());
        }

        #endregion
    }
}
