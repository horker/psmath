using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Horker.DataAnalysis;
using System.Management.Automation;

namespace Tests
{
    [TestClass]
    public class MatrixCmdletsTest
    {
        [TestMethod]
        public void TestMethodAdjust()
        {
            using (var ps = PowerShell.Create()) {
                var ci = new CmdletInfo("Get-Matrix.Add", typeof(GetMatrixAdd));
                ps.AddCommand(ci);
                ps.AddParameters(new Dictionary<string, object>() {
                    { "Lhs", new double[] { 1, 2, 3 } },
                    { "Rhs", 99 }
                });
                var results = ps.Invoke();

                Assert.AreEqual(1, results.Count);
                var matrix = (Matrix)results[0].BaseObject;

                Assert.AreEqual(1, matrix.RowCount);
                Assert.AreEqual(3, matrix.ColumnCount);
                Assert.AreEqual(100, matrix[0, 0]);
            }
        }

        [TestMethod]
        public void TestMethodAdd()
        {
            using (var ps = PowerShell.Create()) {
                var ci = new CmdletInfo("Get-Matrix.Add", typeof(GetMatrixAdd));
                ps.AddCommand(ci);
                ps.AddParameters(new Dictionary<string, object>() {
                    { "Lhs", new double[] { 1, 2, 3 } },
                    { "Rhs", new double[] { 4, 5, 6 } }
                });
                var results = ps.Invoke();

                Assert.AreEqual(1, results.Count);
                var matrix = (Matrix)results[0].BaseObject;

                Assert.AreEqual(1, matrix.RowCount);
                Assert.AreEqual(3, matrix.ColumnCount);
                Assert.AreEqual(5, matrix[0, 0]);
            }
        }

        [TestMethod]
        public void TestMethodDot()
        {
            using (var ps = PowerShell.Create()) {
                var ci = new CmdletInfo("Get-Matrix.Dot", typeof(GetMatrixDot));
                ps.AddCommand(ci);
                ps.AddParameters(new Dictionary<string, object>() {
                    { "Lhs", Matrix.Create(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3, 3) },
                    { "Rhs", 10 }
                });
                var results = ps.Invoke();

                Assert.AreEqual(1, results.Count);
                var matrix = (Matrix)results[0].BaseObject;

                Assert.AreEqual(3, matrix.RowCount);
                Assert.AreEqual(1, matrix.ColumnCount);
                Assert.AreEqual(2 * 10 + 5 * 10 + 8 * 10, matrix[1, 0]);
            }
        }

        [TestMethod]
        public void TestLuDecomposition()
        {
            using (var ps = PowerShell.Create()) {
                var matrix = Matrix.Create(new double[] { 4, 6, 3, 3 }, 2, 2);

                var ci = new CmdletInfo("Get-Matrix.LuDecomposition", typeof(GetMatrixLuDecomposition));
                ps.AddCommand(ci);
                ps.AddParameters(new Dictionary<string, object>() {
                    { "Value",  matrix }
                });
                var results = ps.Invoke();

                Assert.AreEqual(1, results.Count);
                var lu = (LuWrapper)results[0].BaseObject;

                Assert.IsTrue(Accord.Math.Matrix.IsUpperTriangular<double>(lu.U));
                Assert.IsTrue(Accord.Math.Matrix.IsLowerTriangular<double>(lu.L));

                var m = Accord.Math.Matrix.Dot(lu.L, lu.U);

                // To avoid errors caused by floating-point arithmetics
                Accord.Math.Matrix.Apply(m, x => Math.Round(x, 1), m);

                Assert.AreEqual(matrix, (Matrix)m);
            }
        }
    }
}
