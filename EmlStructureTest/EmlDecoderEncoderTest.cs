using EmlStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmlStructureTest
{
    [TestClass]
    public class EmlDecoderEncoderTest
    {
        [TestMethod]
        public void Decode_AsExpected()
        {
            // Prepare.
            string textToDecode = @"=C3=A2me vulgaire.";

            string expectedResult = @"âme vulgaire.";

            // Decode.
            EmlDecoderEncoder decoder = new EmlDecoderEncoder();
            string result = decoder.Decode(textToDecode, EmlDecoderEncoder.Utf8Str, EmlDecoderEncoder.QuotedPrintableStr);

            // Assert.
            Assert.AreEqual(expectedResult, result);
        }
    }
}
