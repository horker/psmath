using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Horker.Math;
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

            Assert.AreEqual(123, d.GetRow(0).Properties["abc"].Value);
            Assert.AreEqual("bbb", d.GetRow(1).Properties["xyz"].Value);
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

        [TestMethod]
        public void TestExpandToOneHot()
        {
            var df = new DataFrame();
            var column = new DataFrameColumn<int>(null, new int[] { 1, 2, 3 });
            df.DefineNewColumn("a", column);

            df.ExpandToOneHot("a", 4);

            Assert.AreEqual(3, df.RowCount);
            Assert.AreEqual(4, df.ColumnCount);

            Assert.AreEqual(0, df["a_0"].GetObject(0));
            Assert.AreEqual(1, df["a_1"].GetObject(0));
            Assert.AreEqual(0, df["a_2"].GetObject(0));
            Assert.AreEqual(0, df["a_3"].GetObject(0));

            Assert.AreEqual(0, df["a_0"].GetObject(1));
            Assert.AreEqual(0, df["a_1"].GetObject(1));
            Assert.AreEqual(1, df["a_2"].GetObject(1));
            Assert.AreEqual(0, df["a_3"].GetObject(1));

            Assert.AreEqual(0, df["a_0"].GetObject(2));
            Assert.AreEqual(0, df["a_1"].GetObject(2));
            Assert.AreEqual(0, df["a_2"].GetObject(2));
            Assert.AreEqual(1, df["a_3"].GetObject(2));
        }

        [TestMethod]
        public void TestWiden()
        {
            var df = new DataFrame();

            df.AddColumn("x", new int[] { 1, 2, 3 });
            df.AddColumn("y", new string[] { "a", "b", "c" });

            var result = df.Widen(4, 5, new string[] { "x", "y" });

            CollectionAssert.AreEqual(new string[] { "x_0", "y_0", "x_1", "y_1", "x_2", "y_2", "x_3", "y_3" }, result.ColumnNames);

            Assert.AreEqual(1, result.GetColumn(0).Count);
            Assert.AreEqual(1, result.GetColumn(1).Count);
            Assert.AreEqual(1, result.GetColumn(2).Count);
            Assert.AreEqual(1, result.GetColumn(3).Count);
            Assert.AreEqual(1, result.GetColumn(4).Count);
            Assert.AreEqual(1, result.GetColumn(5).Count);
            Assert.AreEqual(1, result.GetColumn(6).Count);
            Assert.AreEqual(1, result.GetColumn(7).Count);

            Assert.AreEqual(1, result["x_0"].GetObject(0));
            Assert.AreEqual("a", result["y_0"].GetObject(0));
            Assert.AreEqual(2, result["x_1"].GetObject(0));
            Assert.AreEqual("b", result["y_1"].GetObject(0));
            Assert.AreEqual(3, result["x_2"].GetObject(0));
            Assert.AreEqual("c", result["y_2"].GetObject(0));
            Assert.AreEqual(0, result["x_3"].GetObject(0));
            Assert.AreEqual(null, result["y_3"].GetObject(0));
        }

        [TestMethod]
        public void TestConcatenate()
        {
            var df1 = new DataFrame();
            df1.AddColumn("x", new int[] { 1, 2 });
            df1.AddColumn("y", new string[] { "a", "b" });

            var df2 = new DataFrame();
            df2.AddColumn("x", new int[] { 3, 4 });
            df2.AddColumn("z", new string[] { "c", "d" });

            var result = DataFrame.Concatenate(df1, df2);

            CollectionAssert.AreEqual(new string[] { "x", "y", "z" }, result.ColumnNames);

            Assert.AreEqual(4, result[0].Count);
            Assert.AreEqual(4, result[1].Count);
            Assert.AreEqual(4, result[2].Count);

            CollectionAssert.AreEqual(new object[] { 1, 2, 3, 4 }, result[0].ToObjectArray());
            CollectionAssert.AreEqual(new object[] { "a", "b", null, null }, result[1].ToObjectArray());
            CollectionAssert.AreEqual(new object[] { null, null, "c", "d" }, result[2].ToObjectArray());
        }

        [TestMethod]
        public void TestGroupBy()
        {
            var df = new DataFrame();

            df.AddColumn("x", new string[] { "1", "2", "3", "2" });
            df.AddColumn("y", new string[] { "a", "b", "c", "d" });

            var g = df.GroupBy("x");

            CollectionAssert.AreEqual(new object[] { "1", "2", "3" }, g.Keys);

            CollectionAssert.AreEqual(new string[] { "x", "y" }, g["1"].ColumnNames);
            CollectionAssert.AreEqual(new string[] { "x", "y" }, g["2"].ColumnNames);
            CollectionAssert.AreEqual(new string[] { "x", "y" }, g["3"].ColumnNames);

            CollectionAssert.AreEqual(new object[] { "1" }, g["1"].GetColumn("x").ToObjectArray());
            CollectionAssert.AreEqual(new object[] { "a" }, g["1"].GetColumn("y").ToObjectArray());

            CollectionAssert.AreEqual(new object[] { "2", "2" }, g["2"].GetColumn("x").ToObjectArray());
            CollectionAssert.AreEqual(new object[] { "b", "d" }, g["2"].GetColumn("y").ToObjectArray());

            CollectionAssert.AreEqual(new object[] { "3" }, g["3"].GetColumn("x").ToObjectArray());
            CollectionAssert.AreEqual(new object[] { "c" }, g["3"].GetColumn("y").ToObjectArray());
        }

        [TestMethod]
        public void TestPivot()
        {
            var df = new DataFrame();

            df.AddColumn("x", new string[] { "a", "b", "a", "b", "c" });
            df.AddColumn("y", new int[] { 10, 10, 20, 20, 20 });
            df.AddColumn("v1", new double[] { 1, 2, 3, 4, 5 });
            df.AddColumn("v2", new string[] { "foo", "bar", "baz", "qux", "quux" });

            var p = df.Pivot("x", "y", new string[] { "v1", "v2" });

            CollectionAssert.AreEqual(new string[] { "x", "10_v1", "10_v2", "20_v1", "20_v2" }, p.ColumnNames);
            Assert.AreEqual(3, p.RowCount);

            CollectionAssert.AreEqual(new object[] { "a", "b", "c" }, p["x"].ToObjectArray());
            CollectionAssert.AreEqual(new object[] { 1.0, 2.0, 0.0 }, p["10_v1"].ToObjectArray());
            CollectionAssert.AreEqual(new object[] { "baz", "qux", "quux" }, p["20_v2"].ToObjectArray());
        }
    }
}
