using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TX.Test
{
    public class GridTests
    {
        private Grid<int> grid;

        [SetUp]
        public void SetUp()
        {
            grid = new Grid<int>(new int[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            });
        }

        [Test]
        public void TestTryGet()
        {
            int output;
            var index = new Vec2i(1, 2);
            Assert.IsTrue(grid.TryGet(index, out output));
            Assert.AreEqual(6, output);

            index = new Vec2i(99, 99);
            Assert.IsFalse(grid.TryGet(index, out output));
            Assert.AreEqual(default(int), output);
        }

        [Test]
        public void TestClear()
        {
            grid.Clear();
            foreach (Vec2i c in grid.LocalRegion)
            {
                Assert.AreEqual(0, grid[c]);
            }

            SetUp();
            grid.Clear(1);
            foreach (Vec2i c in grid.LocalRegion)
            {
                Assert.AreEqual(1, grid[c]);
            }
        }
    }
}
