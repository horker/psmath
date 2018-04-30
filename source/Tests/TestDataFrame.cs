using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Horker.DataAnalysis;
using System.Management.Automation;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class TestDataFrame
    {
        [TestMethod]
        public void TestAddRow()
        {
            var d = new DataFrame();

            Assert.AreEqual(0, d.Count);

            var obj = new PSObject();
            obj.Properties.Add(new PSNoteProperty("abc", 123));
            obj.Properties.Add(new PSNoteProperty("xyz", "aaa"));
            d.AddRow(obj);

            var obj2 = new PSObject();
            obj2.Properties.Add(new PSNoteProperty("abc", 456));
            obj2.Properties.Add(new PSNoteProperty("xyz", "bbb"));
            d.AddRow(obj2);

            Assert.AreEqual(2, d.Count);

            Assert.AreEqual(123, ((PSObject)d[0]).Properties["abc"].Value);
            Assert.AreEqual("bbb", ((PSObject)d[1]).Properties["xyz"].Value);
        }

        [TestMethod]
        public void TestForeach()
        {
            var d = new DataFrame();

            d.AddColumn("foo", new object[] { 1, 2, 3, 4 });

            Assert.AreEqual(4, d.Count);

            int c = 1;
            foreach (PSObject e in d) {
                Assert.AreEqual(c, e.Properties["foo"].Value);
                ++c;
            }
        }
    }
}
