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

            var s = Horker.Math.ArrayMethods.AdditionalMethods.SplitInternal(array, new double[] { .6, 2 });

            Assert.AreEqual(3, s.Length);

            Assert.AreEqual(6, s[0].Length);
            Assert.AreEqual(2, s[1].Length);
            Assert.AreEqual(2, s[2].Length);

            CollectionAssert.AreEqual(new object[] { 0, 1, 2, 3, 4, 5 }, s[0]);
            CollectionAssert.AreEqual(new object[] { 6, 7 }, s[1]);
            CollectionAssert.AreEqual(new object[] { 8, 9 }, s[2]);
        }
    }
}
