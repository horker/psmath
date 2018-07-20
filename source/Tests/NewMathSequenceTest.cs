using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Horker.Math;
using System.Management.Automation;

namespace Tests
{
    [TestClass]
    public class NewMathSequenceTest
    {
        [TestMethod]
        public void TestSequence()
        {
            using (var ps = PowerShell.Create())
            {
                var ci = new CmdletInfo("New-Math.Sequence", typeof(NewMathSequence));
                ps.AddCommand(ci);
                ps.AddParameters(new object[] { 10, 3 });
                var results = ps.Invoke();

                Assert.AreEqual(4, results.Count);

                Assert.AreEqual(0, results[0]);
                Assert.AreEqual(3, results[1]);
                Assert.AreEqual(6, results[2]);
                Assert.AreEqual(9, results[3]);
            }
        }
    }
}
