using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TX.Test
{
    public class RotationExtTests
    {
        [Test]
        public void TestAdd()
        {
            Assert.AreEqual(Rot4.None, Rot4.None.Add(Rot4.None));
            Assert.AreEqual(Rot4.CCW90, Rot4.CCW90.Add(Rot4.None));
            Assert.AreEqual(Rot4.Rev, Rot4.Rev.Add(Rot4.None));
            Assert.AreEqual(Rot4.CW90, Rot4.CW90.Add(Rot4.None));

            Assert.AreEqual(Rot4.CCW90, Rot4.None.Add(Rot4.CCW90));
            Assert.AreEqual(Rot4.Rev, Rot4.CCW90.Add(Rot4.CCW90));
            Assert.AreEqual(Rot4.CW90, Rot4.Rev.Add(Rot4.CCW90));
            Assert.AreEqual(Rot4.None, Rot4.CW90.Add(Rot4.CCW90));

            Assert.AreEqual(Rot4.Rev, Rot4.None.Add(Rot4.Rev));
            Assert.AreEqual(Rot4.CW90, Rot4.CCW90.Add(Rot4.Rev));
            Assert.AreEqual(Rot4.None, Rot4.Rev.Add(Rot4.Rev));
            Assert.AreEqual(Rot4.CCW90, Rot4.CW90.Add(Rot4.Rev));

            Assert.AreEqual(Rot4.CW90, Rot4.None.Add(Rot4.CW90));
            Assert.AreEqual(Rot4.None, Rot4.CCW90.Add(Rot4.CW90));
            Assert.AreEqual(Rot4.CCW90, Rot4.Rev.Add(Rot4.CW90));
            Assert.AreEqual(Rot4.Rev, Rot4.CW90.Add(Rot4.CW90));
        }
    }
}
