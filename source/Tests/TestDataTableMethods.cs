using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Horker.Math;
using System.Management.Automation;
using System.Collections.Generic;
using System.Data;

namespace Tests
{
    [TestClass]
    public class TestDataTable
    {
        [TestMethod]
        public void TestExpandToOneHot()
        {
            var table = new DataTable();
            var column = new DataColumn("a")
            {
                DataType = typeof(Int32)
            };
            table.Columns.Add(column);

            column = new DataColumn("b")
            {
                DataType = typeof(Int32),
                DefaultValue = 0
            };
            table.Columns.Add(column);

            var row = table.NewRow();
            row["a"] = 1;
            table.Rows.Add(row);

            row = table.NewRow();
            row["a"] = 2;
            table.Rows.Add(row);

            row = table.NewRow();
            row["a"] = 3;
            table.Rows.Add(row);

            table.ExpandToOneHot("a", 4);

            Assert.IsTrue(table.Columns.Contains("a_0"));
            Assert.IsTrue(table.Columns.Contains("a_1"));
            Assert.IsTrue(table.Columns.Contains("a_2"));
            Assert.IsTrue(table.Columns.Contains("a_3"));
            Assert.IsFalse(table.Columns.Contains("a_4"));

            Assert.AreEqual(0, table.Rows[0]["a_0"]);
            Assert.AreEqual(1, table.Rows[0]["a_1"]);
            Assert.AreEqual(0, table.Rows[0]["a_2"]);
            Assert.AreEqual(0, table.Rows[0]["a_3"]);

            Assert.AreEqual(0, table.Rows[1]["a_0"]);
            Assert.AreEqual(0, table.Rows[1]["a_1"]);
            Assert.AreEqual(1, table.Rows[1]["a_2"]);
            Assert.AreEqual(0, table.Rows[1]["a_3"]);

            Assert.AreEqual(0, table.Rows[2]["a_0"]);
            Assert.AreEqual(0, table.Rows[2]["a_1"]);
            Assert.AreEqual(0, table.Rows[2]["a_2"]);
            Assert.AreEqual(1, table.Rows[2]["a_3"]);
        }
    }
}
