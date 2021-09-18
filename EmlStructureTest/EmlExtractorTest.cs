using EmlStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace EmlStructureTest
{
    /// <summary>
    /// Summary description for EmlExtractorTest
    /// </summary>
    [TestClass]
    public class EmlExtractorTest
    {
        private readonly Dictionary<string, string>
           mEncodedStringToDecodedStringDict = new Dictionary<string, string>
       {
               {
                   "\"=?UTF-8?B?MDIzIOKFoS0z44CA5Lit5aSu44K744Oz44K/44O85YaF6KaW6Y+hLg==?=\r\n =?UTF-8?B?5L+u5q2jLmRvY20=?=\"",
                   "023 Ⅱ-3　中央センター内視鏡.修正.docm"
               },

               {
                   "=?iso-8859-1?Q?=A1Hola,_se=F1or!?=","¡Hola, señor!"
               }
       };

        [TestMethod]
        public void DecodeName_AsExpected()
        {
            foreach (KeyValuePair<string, string> keyValuePairs in mEncodedStringToDecodedStringDict)
            {
                string decodedName = (new EmlExtractor()).DecodeName(keyValuePairs.Key);
                Assert.AreEqual(
                    decodedName,
                    keyValuePairs.Value,
                    $"Failed encoding {keyValuePairs.Key}");
            }
        }
    }
}
