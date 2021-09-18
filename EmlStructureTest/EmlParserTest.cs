using EmlStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmlStructureTest
{
    [TestClass]
    public class EmlParserTest
    {
        private const string EmlToParse = @"Resources/OrginialBigEML.eml";
        private const string EmlToValidateParsing = @"Resources/ParsedBigEML_ToCompareWith.eml";
        private const string tempEml = @"Resources/tempParsed";

        [TestMethod]
        public void Parse_ValidateParsing()
        {
            try
            {
                // Prepare.
                EmlParser emlParser = new EmlParser();

                // Parse.
                string emlText = File.ReadAllText(EmlToParse, Encoding.Default);
                IParsable emlData = emlParser.Parse(emlText);
                emlData.Save(tempEml);

                // Test.
                string parsedEmlToValidateText = File.ReadAllText(EmlToValidateParsing);
                string testedEmlToText = File.ReadAllText(tempEml);
                Assert.AreEqual(
                    parsedEmlToValidateText,
                    testedEmlToText);
            }
            finally
            {
                File.Delete(tempEml);
            }
        }

        [TestMethod]
        public void ParseHeader_ValidateParsing()
        {
            // Prepare.
            EmlParser emlParser = new EmlParser();

            string headerContent= "Content-Type: text/html; charset=\"utf-8\"\r\nContent-Transfer-Encoding: quoted-printable\r\n\r\nJ'interdis aux marchands de vanter trop leur marchandises.";
            string[] valuesToValidate = new string[] 
            {
                "Content-Type", "text/html; charset=\"utf-8\"",
                "Content-Transfer-Encoding", "quoted-printable"
            };

            // Parse.
            Dictionary<string, List<string>> HeadersKeysValues = emlParser.ExtractHeadersKeysValues(headerContent);

            // Test.
            int validateIndex = 0;
            foreach(KeyValuePair<string, List<string>> header in HeadersKeysValues)
            {
                Assert.AreEqual(header.Key, valuesToValidate[validateIndex], $"{header.Key} != {valuesToValidate[validateIndex]}");
                validateIndex++;
                Assert.AreEqual(header.Value[0], valuesToValidate[validateIndex], $"{header.Key} != {valuesToValidate[validateIndex]}");
                validateIndex++;
            }
        }
    }
}
