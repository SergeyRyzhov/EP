using ChipSynthesys.Common.Randoms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ChipSynthesys.UnitTests.Common
{
    [TestClass]
    public class TableRandomTests
    {
        [TestMethod]
        public void MathEpectationTest()
        {
            IRandom<int> random;

            {
                var tableRandom = new TableRandom<int>();
                tableRandom.Add(1, 40);
                tableRandom.Add(2, 50);
                tableRandom.Add(3, 10);
                random = tableRandom;
            }

            var actual = random.MathematicalExpectation();

            Console.WriteLine(actual);
            Assert.AreEqual(1.7, actual);
        }

        [TestMethod]
        public void MathEpectationForOrdinalTest()
        {
            IRandom<int> random;

            {
                var tableRandom = new TableRandom<int>();
                tableRandom.Add(1, 25);
                tableRandom.Add(2, 25);
                tableRandom.Add(3, 25);
                tableRandom.Add(4, 25);
                random = tableRandom;
            }

            var actual = random.MathematicalExpectation();

            Console.WriteLine(actual);
            Assert.AreEqual(2.5, actual);
        }
    }
}