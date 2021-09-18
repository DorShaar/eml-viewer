using EmlStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace EmlStructureTest
{
    [TestClass]
    public class ContentTypeInfoTest
    {
        private readonly Dictionary<string, string[]>
            mContentTypesDataToContentTypeValuesDict = new Dictionary<string, string[]>
        {
                {
                    " multipart/mixed;\r\n\tboundary=\"----=_NextPart_000_0004_01CD41C2.02B6AEC0\"\r\n",
                    new string[] { "multipart", "mixed", "boundary", "----=_NextPart_000_0004_01CD41C2.02B6AEC0" }
                },

                {
                    " image/gif",
                    new string[] { "image", "gif" }
                },

                {
                    " text/css;\r\n\tcharset=\"iso-2022-jp\"",
                    new string[] { "text", "css", "charset", "iso-2022-jp" }
                },

                {
                    " text/plain; charset=\"iso-2022-jp\"; format=flowed; delsp=yes",
                    new string[] { "text", "plain", "charset", "iso-2022-jp", "format", "flowed", "delsp", "yes"}
                },

                {
                    " text/calendar; Method=\"REQUEST\"; Name=\"meeting.ics\"; charset=\"utf-8\"",
                    new string[] { "text", "calendar", "Method", "REQUEST", "Name", "meeting.ics", "charset", "utf-8" }
                },

                {
                    " application/x-zip-compressed;\r\nname=\"I'd like to add you to my professional network on LinkedIn.zip\"",
                    new string[] { "application", "x-zip-compressed", "name", "I'd like to add you to my professional network on LinkedIn.zip" }
                }
        };

        [TestMethod]
        public void UpdateContentTypeInfo_ValidContentInfo_AsExpected()
        {
            foreach (KeyValuePair<string, string[]> keyValuePairs in mContentTypesDataToContentTypeValuesDict)
            {
                ContentTypeInfo contentTypeInfo = new ContentTypeInfo(keyValuePairs.Key);
                Assert.IsTrue(
                    CompareContentTypeInfoToValue(contentTypeInfo, keyValuePairs.Value),
                    $"Failed Parsing {keyValuePairs.Key}");
            }
        }

        private bool CompareContentTypeInfoToValue(ContentTypeInfo contentTypeInfo, string[] values)
        {
            bool areEqual = true;

            // Validating Type and SubType.
            if (!contentTypeInfo.Type.Equals(values[0]) ||
                !contentTypeInfo.SubType.Equals(values[1]))
            {
                areEqual = false;
            }

            if (areEqual)
            {
                for (int i = 2; i < values.Length; i += 2)
                {
                    string parameterAttribute = values[i];
                    string parameterValue = values[i + 1];
                    if (!contentTypeInfo.GetParameterValue(parameterAttribute).Equals(parameterValue))
                    {
                        areEqual = false;
                    }
                }
            }

            return areEqual;
        }
    }
}
