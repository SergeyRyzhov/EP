using ChipSynthesys.Common.Randoms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ChipSynthesys.UnitTests.Common
{
    [TestClass]
    public class RangeRandomTests
    {
        [TestMethod]
        public void MathEpectationTest()
        {
            IRandom<int> random = new RangeRandom(3, 6, new Random());
            var actual = random.MathematicalExpectation();
            Console.WriteLine(actual);
            Assert.AreEqual(4.5, actual);
        }
    }
}