using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace EmlStructure
{
    public class EmlExtractor : IDataExtractor<object>
    {
        private const string FileNameAttribute = "filename=";

        /// <summary>
        /// Returns decoded name.
        /// Searches file name attribute from Content-Type, Content-Disposition.
        /// In case file name no found, returns empty string.
        /// </summary>
        /// <returns></returns>
        public string GetName(object itemData)
        {
            string validName = string.Empty;
            if (itemData is EmlLeafNode emlNode)
            {
                validName = DecodeName(emlNode.Header.ContentType.GetParameterValue("name"));

                // Try to take name from Content Disposition.
                if (validName.Equals(string.Empty))
                {
                    List<string> splittedContentDisposition = ExtendedSplitter.Split(
                        emlNode.Header.ContentDisposition,
                        new string[] { FileNameAttribute });

                    if (splittedContentDisposition.Count >= 2)
                    {
                        validName = DecodeName(splittedContentDisposition[1].RemoveInvalidPathCharacters());
                    }
                }
            }

            return validName;
        }

        public void Extract(object itemData, string filePath)
        {
            if (itemData is EmlLeafNode emlNode)
            {
                if (emlNode.Header.ContentTransferEncoding.Contains(EmlDecoderEncoder.Base64Str))
                {
                    byte[] dataBytes = Convert.FromBase64String(emlNode.Body);
                    dataBytes.SaveToHardDrive(filePath);
                }
                else
                    emlNode.Save(filePath);
            }
        }

        /// <summary>
        /// Non-ASCII data: The form is: "=?charset?encoding?encoded text?=".
        /// * Charset may be any character set registered with IANA.
        ///     Typically it would be the same charset as the message body.
        /// * Encoding can be either "Q" denoting Q-encoding that is similar
        ///     to the quoted-printable encoding, or "B" denoting base64
        ///     encoding.
        /// * encoded text is the Q-encoded or base64-encoded text.
        /// * An encoded-word may not be more than 75 characters long,
        ///     including charset, encoding, encoded text, and delimiters.
        ///     If it is desirable to encode more text than will fit in
        ///     an encoded-word of 75 characters, multiple encoded-words 
        ///     (separated by CRLF SPACE) may be used.
        /// </summary>
        internal string DecodeName(string name)
        {
            EmlDecoderEncoder decoder = new EmlDecoderEncoder();
            StringBuilder decodedName = new StringBuilder();
            decodedName.Append(name);

            if (decoder.IsValidEncodingFormat(name))
            {
                List<string> contentTypeParts = ExtendedSplitter.Split(name, decoder.EncodedTextFormat);

                // Extracts charset.
                string charset = contentTypeParts[1];

                // Extracts encoding.
                string encoding = contentTypeParts[2];

                // Extracts encoded text.
                string encodedText = contentTypeParts[3];

                // Decode encoded text.
                decodedName.Clear();
                decodedName.Append(decoder.Decode(encodedText, charset, encoding));
                if (contentTypeParts.Count > 3)
                {
                    decodedName.Append(DecodeName(contentTypeParts[4]));
                }
            }

            return decodedName.ToString().RemoveInvalidPathCharacters();
        }
    }
}