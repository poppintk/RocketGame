using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TX.Test
{
    public class UnorderedPairTests
    {
        [Test]
        public void EqualsTest()
        {
            Assert.AreEqual(
                new UnorderedPair<int>(1, 1),
                new UnorderedPair<int>(1, 1));

            Assert.AreEqual(
                new UnorderedPair<int>(1, 2),
                new UnorderedPair<int>(2, 1));
        }

        [Test]
        public void GetHashCodeTest()
        {
            Assert.AreEqual(
                new UnorderedPair<int>(1, 1).GetHashCode(),
                new UnorderedPair<int>(1, 1).GetHashCode());

            Assert.AreEqual(
                new UnorderedPair<int>(1, 2).GetHashCode(),
                new UnorderedPair<int>(2, 1).GetHashCode());
        }

        [Test]
        public void NotNullTest()
        {
            Assert.Catch<NullReferenceException>(() => new UnorderedPair<object>(null, 42));
            Assert.Catch<NullReferenceException>(() => new UnorderedPair<object>(42, null));
            Assert.Catch<NullReferenceException>(() => new UnorderedPair<object>(null, null));
        }
    }
}
