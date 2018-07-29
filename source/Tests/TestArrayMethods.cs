using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TestArrayMethods
    {
        [TestMethod]
        public void TestSplit()
        {
            var array = new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var s = Horker.Math.ArrayMethods.Typed.Additional<object>.SplitInternal(array, new object[] { .6, 2 });

            Assert.AreEqual(3, s.Length);

            Assert.AreEqual(6, s[0].Length);
            Assert.AreEqual(2, s[1].Length);
            Assert.AreEqual(2, s[2].Length);

            CollectionAssert.AreEqual(new object[] { 0, 1, 2, 3, 4, 5 }, s[0]);
            CollectionAssert.AreEqual(new object[] { 6, 7 }, s[1]);
            CollectionAssert.AreEqual(new object[] { 8, 9 }, s[2]);
        }

        [TestMethod]
        public void TestSlice1()
        {
            var array = new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var s = Horker.Math.ArrayMethods.Typed.Additional<object>.SliceInternal(array, new object[] { 1, 5, 9 });

            Assert.AreEqual(3, s.Length);

            CollectionAssert.AreEqual(new object[] { 1, 5, 9 }, s);
        }

        [TestMethod]
        public void TestSlice2()
        {
            var array = new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var s = Horker.Math.ArrayMethods.Typed.Additional<object>.SliceInternal(array, new object[] { 1, new int []{ 5, 7 }, 9 });

            Assert.AreEqual(4, s.Length);

            CollectionAssert.AreEqual(new object[] { 1, 5, 6, 9 }, s);
        }
    }
}
