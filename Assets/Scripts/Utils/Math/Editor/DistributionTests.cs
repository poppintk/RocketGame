using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TX.Test
{
    internal class WeightedSelectionTests
    {
        [Test]
        public void SelectItemTest()
        {
            int[] weights = new[] { 2, 3, 5, 7 };
            WeightedSelection<int> ws = new WeightedSelection<int>(
                weights.ToList(),
                i => i);

            Assert.AreEqual(17, ws.TotalWeights);
            CollectionAssert.AreEquivalent(weights, ws.Items);
            CollectionAssert.AreEquivalent(new[] { 2, 5, 10, 17 }, ws.WeightCDF);

            Assert.AreEqual(2, ws.SelectItem(1));
            Assert.AreEqual(2, ws.SelectItem(2));

            Assert.AreEqual(3, ws.SelectItem(3));
            Assert.AreEqual(3, ws.SelectItem(4));
            Assert.AreEqual(3, ws.SelectItem(5));

            Assert.AreEqual(5, ws.SelectItem(6));
            Assert.AreEqual(5, ws.SelectItem(7));
            Assert.AreEqual(5, ws.SelectItem(8));
            Assert.AreEqual(5, ws.SelectItem(9));
            Assert.AreEqual(5, ws.SelectItem(10));

            Assert.AreEqual(7, ws.SelectItem(11));
            Assert.AreEqual(7, ws.SelectItem(12));
            Assert.AreEqual(7, ws.SelectItem(13));
            Assert.AreEqual(7, ws.SelectItem(14));
            Assert.AreEqual(7, ws.SelectItem(15));
            Assert.AreEqual(7, ws.SelectItem(16));
            Assert.AreEqual(7, ws.SelectItem(17));

            Assert.Catch<ArgumentOutOfRangeException>(() => ws.SelectItem(18));
        }

        [Test]
        public void RemoveTest()
        {
            int[] weights = new[] { 2, 3, 5, 7 };
            WeightedSelection<int> ws = new WeightedSelection<int>(
                weights.ToList(),
                i => i);

            Assert.AreEqual(17, ws.TotalWeights);
            CollectionAssert.AreEquivalent(weights, ws.Items);
            CollectionAssert.AreEquivalent(new[] { 2, 5, 10, 17 }, ws.WeightCDF);

            ws.Remove(1);
            Assert.AreEqual(14, ws.TotalWeights);
            CollectionAssert.AreEquivalent(new[] { 2, 5, 7 }, ws.Items);
            CollectionAssert.AreEquivalent(new[] { 2, 7, 14 }, ws.WeightCDF);
        }
    }
}
