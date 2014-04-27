using ChipSynthesys.Common.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ChipSynthesys.UnitTests.Common
{
    [TestClass]
    public class ValuePairTests
    {
        [TestMethod]
        public void MathEpectationTest()
        {
            var vp = new ValuePair<int>();
            vp.A = 5;
            vp.B = 7;

            var actual = vp.Length;

            Console.WriteLine(actual);
            Assert.AreEqual(2, actual);
        }
    }
}