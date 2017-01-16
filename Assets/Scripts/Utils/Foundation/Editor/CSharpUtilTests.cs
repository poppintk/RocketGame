using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace TX.Test
{
    [TestFixture]
    public class ReflectionUtilTests
    {
        private class SampleBase
        {
            public readonly string MyString = "String";
            public readonly int MyInt = 42;
            public readonly float MyFloat = 3.14159f;
            public readonly SampleList MyList = new SampleList() { 0, 2, 4, 6, 8 };
        }

        private class SampleChildA : SampleBase { }

        private class SampleChildB : SampleBase { }

        private class SampleList : List<int> { }

        [Test]
        public void TestGetSubType()
        {
            var subTypes = typeof(SampleBase).GetSubTypes();
            CollectionAssert.AreEquivalent(
                new[] { typeof(SampleChildA), typeof(SampleChildB) },
                subTypes);
        }

        [Test]
        public void TestIsSubclassOfRawGeneric()
        {
            Assert.IsFalse(typeof(SampleList).IsSubclassOf(typeof(List<>)));
            Assert.IsTrue(typeof(SampleList).IsSubclassOfRawGeneric(typeof(List<>)));
        }

        [Test]
        public void TestIsArrayOf()
        {
            Assert.IsFalse(typeof(SampleChildA[]).IsArrayOf(typeof(SampleChildB)));
            Assert.IsTrue(typeof(SampleChildA[]).IsArrayOf(typeof(SampleChildA)));
            Assert.IsTrue(typeof(SampleChildA[]).IsArrayOf(typeof(SampleBase)));
        }

        [Test]
        public void TestIsListOf()
        {
            Assert.IsFalse(typeof(List<SampleChildA>).IsListOf(typeof(SampleChildB)));
            Assert.IsTrue(typeof(List<SampleChildA>).IsListOf(typeof(SampleChildA)));
            Assert.IsTrue(typeof(List<SampleChildA>).IsListOf(typeof(SampleBase)));
        }

        [Test]
        public void TestGetFieldValue()
        {
            SampleBase sampleObj = new SampleBase();
            Assert.AreEqual(sampleObj.MyString, ReflectionUtil.GetFieldValue(sampleObj, "MyString"));
            Assert.AreEqual(sampleObj.MyInt, ReflectionUtil.GetFieldValue(sampleObj, "MyInt"));
            Assert.AreEqual(sampleObj.MyFloat, ReflectionUtil.GetFieldValue(sampleObj, "MyFloat"));
            Assert.AreEqual(sampleObj.MyList, ReflectionUtil.GetFieldValue(sampleObj, "MyList"));
            Assert.AreEqual(sampleObj.MyList[2], ReflectionUtil.GetFieldValue(sampleObj, "MyList", 2));
        }

        [Test]
        public void TestGetGenericMethod()
        {
            var createMethod = typeof(ScriptableObject)
                .GetGenericMethod("CreateInstance", null)
                .MakeGenericMethod(typeof(GUISkin));    // or any builtin scriptable object
            object so = createMethod.Invoke(null, null);
            Assert.IsNotNull(so);
        }
    }

    [TestFixture]
    public class EnumUtilTests
    {
        private class SampleAttribute : System.Attribute { }

        private enum SampleEnum
        {
            A,

            [Sample]
            B,

            C,
        }

        [Flags]
        private enum SampleFlags
        {
            NONE = 0,
            F1 = 1 << 0,
            F2 = 1 << 1,
            F3 = 1 << 2,
            ALL = F1 | F2 | F3,
        }

        [Test]
        public void TestGetValues()
        {
            CollectionAssert.AreEqual(
                new[] { SampleEnum.A, SampleEnum.B, SampleEnum.C },
                EnumUtil.GetValues<SampleEnum>());
            CollectionAssert.AreEqual(
                new[] { SampleFlags.NONE, SampleFlags.F1, SampleFlags.F2, SampleFlags.F3, SampleFlags.ALL },
                EnumUtil.GetValues<SampleFlags>());
        }

        [Test]
        public void TestHasFlag()
        {
            Assert.IsTrue(SampleFlags.F1.HasFlag(SampleFlags.NONE));
            Assert.IsTrue(SampleFlags.F1.HasFlag(SampleFlags.F1));
            Assert.IsFalse(SampleFlags.F1.HasFlag(SampleFlags.F2));
            Assert.IsFalse(SampleFlags.F1.HasFlag(SampleFlags.ALL));
            Assert.IsFalse(SampleFlags.NONE.HasFlag(SampleFlags.F1));

            Assert.IsTrue(SampleFlags.ALL.HasFlag(SampleFlags.F1));
            Assert.IsTrue(SampleFlags.ALL.HasFlag(SampleFlags.F2));
            Assert.IsTrue(SampleFlags.ALL.HasFlag(SampleFlags.F3));

            var F13 = (SampleFlags.F1 | SampleFlags.F3);
            Assert.IsTrue(F13.HasFlag(F13));
            Assert.IsTrue(F13.HasFlag(SampleFlags.F1));
            Assert.IsFalse(F13.HasFlag(SampleFlags.F2));
            Assert.IsTrue(F13.HasFlag(SampleFlags.F3));
        }

        [Test]
        public void TestIsFlag()
        {
            Assert.IsFalse(SampleFlags.NONE.IsFlag());
            Assert.IsTrue(SampleFlags.F1.IsFlag());
            Assert.IsFalse(SampleFlags.ALL.IsFlag());
        }

        [Test]
        public void TestGetAttribute()
        {
            Assert.IsNull(SampleEnum.A.GetAttribute<SampleAttribute>());

            var attrib = SampleEnum.B.GetAttribute<SampleAttribute>();
            Assert.IsNotNull(attrib);
            Assert.AreEqual(typeof(SampleAttribute), attrib.GetType());
        }
    }

    [TestFixture]
    public class GenericUtilTests
    {
        [Test]
        public void TestSwap()
        {
            int a = 1, b = 2;
            GenericUtil.Swap(ref a, ref b);
            Assert.AreEqual(2, a);
            Assert.AreEqual(1, b);

            Vector2 va = Vector2.zero, vb = Vector2.one;
            GenericUtil.Swap(ref va, ref vb);
            Assert.AreEqual(Vector2.one, va);
            Assert.AreEqual(Vector2.zero, vb);

            string sa = "a", sb = "b";
            GenericUtil.Swap(ref sa, ref sb);
            Assert.AreEqual("b", sa);
            Assert.AreEqual("a", sb);
        }
    }

    [TestFixture]
    public class StringUtilTests
    {
        [Test]
        public void TestSpaceWord()
        {
            Assert.AreEqual("Iron Pipe", StringUtil.SpaceWords("IronPipe"));
            Assert.AreEqual("Iron Pipe", StringUtil.SpaceWords("ironPipe"));
            Assert.AreEqual("Iron Pipe 123", StringUtil.SpaceWords("ironPipe123"));
        }

        [Test]
        public void TestTitleCase()
        {
            Assert.AreEqual("IronPipe", StringUtil.TitleCase("ironPipe"));
            Assert.AreEqual("Iron pipe 123", StringUtil.TitleCase("iron pipe 123"));
        }
    }

    [TestFixture]
    public class ArrayExtensionsTests
    {
        private int[] array;

        [SetUp]
        public void SetUp()
        {
            array = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        }

        [Test]
        public void TestGetEnumerable()
        {
            // ArraySegment
            var expected = new ArraySegment<int>(array).GetEnumerable().ToList();
            CollectionAssert.AreEquivalent(array, expected);
            CollectionAssert.AreEquivalent(array.Take(2), new ArraySegment<int>(array, 0, 2).GetEnumerable().ToList());
            CollectionAssert.AreEquivalent(array.Skip(2), new ArraySegment<int>(array, 2, array.Length - 2).GetEnumerable().ToList());
            CollectionAssert.AreEquivalent(array.Skip(2).Take(3), new ArraySegment<int>(array, 2, 3).GetEnumerable().ToList());
        }

        [Test]
        public void TestArranged()
        {
            // double the values in array
            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= 2;
            }
            // essentially get the reverse of array
            var arranged = array.Arranged(new[] { 7, 6, 5, 4, 3, 2, 1, 0 });
            CollectionAssert.AreEqual(
                array.Reverse(),
                arranged);
        }
    }

    [TestFixture]
    public class CSharpExtensionsTests
    {
        [Test]
        public void TestBinarySearch()
        {
            List<string> strs = new List<string>
            {
                "",
                "1",
                "11",
                "111",
                "1111",
                "111111"
            };
            for (int i = 0; i < 5; i++)
            {
                var idx = strs.BinarySearch(s => s.Length, i);
                Assert.AreEqual(i, idx);
            }

            Assert.AreEqual(~5, strs.BinarySearch(s => s.Length, 5));
            Assert.AreEqual(5, strs.BinarySearch(s => s.Length, 6));
            Assert.AreEqual(~6, strs.BinarySearch(s => s.Length, 7));
            Assert.AreEqual(~6, strs.BinarySearch(s => s.Length, 8));
            Assert.AreEqual(~6, strs.BinarySearch(s => s.Length, 100));
        }
    }
}
