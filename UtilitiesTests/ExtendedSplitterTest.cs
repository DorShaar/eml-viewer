using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace UtilitiesTest
{
    [TestClass]
    public class ExtendedSplitterTest
    {
        [TestMethod]
        public void Split_AsExpected()
        {
            SplitTest1();
            SplitTest2();
        }

        private void SplitTest1()
        {
            string str = "?ab?=c";
            List<string> expected = new List<string> { "", "ab", "c" };
            List<string> result = ExtendedSplitter.Split(str, new string[] { "?", "?=" });
            CollectionAssert.AreEqual(result, expected);
        }

        private void SplitTest2()
        {
            string str = "xxx?ab?=c";
            List<string> expected = new List<string> { "xxx", "ab", "c" };
            List<string> result = ExtendedSplitter.Split(str, new string[] { "?", "?=" });
            CollectionAssert.AreEqual(result, expected);
        }

        [TestMethod]
        public void SplitByLength_AsExpected()
        {
            string str = "12 345 6 78";
            List<string> expected = new List<string> { "12", "345", "6", "78" };
            List<string> result = ExtendedSplitter.SplitByLength(str, 3);
            CollectionAssert.AreEqual(result, expected);
        }
    }
}
