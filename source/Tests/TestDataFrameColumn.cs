using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Horker.DataAnalysis;
using System.Management.Automation;

namespace Tests
{
    [TestClass]
    public class TestDataFrameColumn
    {
        [TestMethod]
        public void TestCodify()
        {
            var v = new DataFrameColumn {
                "a",
                "b",
                "a",
                "B",
                "C"
            };

            DataFrame df = v.ToDummyValues("Code ");

            Assert.AreEqual(5, df.Count);

            Assert.AreEqual(0, df["Code b"][0]);
            Assert.AreEqual(1, df["Code b"][1]);
            Assert.AreEqual(0, df["Code b"][2]);
            Assert.AreEqual(1, df["Code b"][3]);
            Assert.AreEqual(0, df["Code b"][4]);

            Assert.AreEqual(0, df["Code c"][0]);
            Assert.AreEqual(0, df["Code c"][1]);
            Assert.AreEqual(0, df["Code c"][2]);
            Assert.AreEqual(0, df["Code c"][3]);
            Assert.AreEqual(1, df["Code c"][4]);
        }
/*
        [TestMethod]
        public void TestOuter()
        {
            // |1|             | (1 * 4) (1 * 5) |   |  4  5 |
            // |2| x [ 4 5 ] = | (2 * 4) (2 * 5) | = |  8 10 |
            // |3|             | (3 * 4) (3 * 5) |   | 12 15 |

            var v1 = new Vector {
                1, 2, 3
            };
            var v2 = new Vector {
                4, 5
            };

            var df = v1.Outer(v2);

            Assert.AreEqual(df.RowCount, 3);
            Assert.AreEqual(df.ColumnCount, 2);

            Assert.AreEqual(df[0, 0], 1.0 * 4);
            Assert.AreEqual(df[1, 1], 2.0 * 5);
            Assert.AreEqual(df[2, 1], 3.0 * 5);
        }
*/
    }
}
