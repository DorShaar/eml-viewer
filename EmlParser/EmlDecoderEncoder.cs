using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace EmlStructure
{
    public class EmlDecoderEncoder : ITextEncoderDecoder
    {
        // Encoded Text.
        public readonly string[] EncodedTextFormat = new string[]
        {
            CharsetPrefix,
            EncodingPerfix,
            EncodedTextPrefix,
            EndcodedTextSuffix
        };

        public const string CharsetPrefix = "=?";
        public const string EncodingPerfix = "?";
        public const string EncodedTextPrefix = "?";
        public const string EndcodedTextSuffix = "?=";

        // Encoding Variables.
        public const string QuotedPrintableEncodingCode = "Q";
        public const string Base64EncodingCode = "B";
        public const string Base64Str = "base64";
        public const string Utf8Str = "utf-8";
        public const string SevenBitStr = "7bit";
        public const string QuotedPrintableStr = "quoted-printable";

        private const string NotDecodeableTextMessage = "--Not decodeable text --";

        public string Decode(string encodedText, string charset, string encoding)
        {
            string decodedText = NotDecodeableTextMessage;

            if (!string.IsNullOrEmpty(charset))
            {
                if(encoding.Equals(string.Empty))
                {
                    Encoding encodingFromCharset = Encoding.GetEncoding(charset);
                    byte[] dataBytes = encodingFromCharset.GetBytes(encodedText);
                    decodedText = GetTextByCharset(dataBytes, charset);
                }
                else
                {
                    // Q-encoding is quoted-printable.
                    if (encoding.Equals(QuotedPrintableEncodingCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        StringBuilder reassambledNameText = new StringBuilder();
                        reassambledNameText.Append(CharsetPrefix);
                        reassambledNameText.Append(charset);
                        reassambledNameText.Append(EncodingPerfix);
                        reassambledNameText.Append(encoding);
                        reassambledNameText.Append(EncodedTextPrefix);
                        reassambledNameText.Append(encodedText);
                        reassambledNameText.Append(EndcodedTextSuffix);

                        decodedText = System.Net.Mail.Attachment.CreateAttachmentFromString(
                            string.Empty, reassambledNameText.ToString()).Name;
                    }
                    // Base64 encoding.
                    else if (encoding.Equals(Base64EncodingCode, StringComparison.InvariantCultureIgnoreCase) ||
                             encoding.Equals(Base64Str, StringComparison.InvariantCultureIgnoreCase))
                    {
                        byte[] dataBytes = Convert.FromBase64String(encodedText);
                        decodedText = GetTextByCharset(dataBytes, charset);
                    }
                    // 7 bit (ASCII).
                    else if (encoding.Equals(SevenBitStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        byte[] dataBytes = Encoding.ASCII.GetBytes(encodedText);
                        decodedText = GetTextByCharset(dataBytes, charset);
                    }
                    // Quoted-Printable encoding.
                    else if (encoding.Equals(QuotedPrintableStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        byte[] dataBytes = GetQuotedPrintableBytes(encodedText);
                        decodedText = GetTextByCharset(dataBytes, charset);
                    }
                }
            }

            return decodedText;
        }

        private string GetTextByCharset(byte[] dataBytes, string charset)
        {
            string textFromBytes = string.Empty;

            Encoding encodingByCharset = Encoding.GetEncoding(charset);
            textFromBytes = encodingByCharset.GetString(dataBytes);
            return textFromBytes;
        }

        private byte[] GetQuotedPrintableBytes(string encodedText)
        {
            int i = 0;
            List<byte> byteArray = new List<byte>();
            while (i < encodedText.Length)
            {
                // Soft Line breaks.
                if (encodedText[i] == EmlVariables.EqualSign && encodedText[i + 1] == '\r' && encodedText[i + 2] == '\n')
                {
                    // Skips.
                    i += 3;
                }
                else if (encodedText[i] == EmlVariables.EqualSign)
                {
                    string subString = encodedText.Substring(i + 1, 2);
                    int hex = Convert.ToInt32(subString, fromBase: 16);
                    byte byt = Convert.ToByte(hex);
                    byteArray.Add(byt);
                    i += 3;
                }
                else
                {
                    byteArray.Add((byte)encodedText[i]);
                    i++;
                }
            }

            return byteArray.ToArray();
        }

        public string Decode(object itemData)
        {
            string decodedText = NotDecodeableTextMessage;
            if (itemData is EmlLeafNode emlNode)
            {
                string textToDecode = emlNode.Body;
                string charset = emlNode.Header.ContentType.Charset;
                string encoding = emlNode.Header.ContentTransferEncoding;
                decodedText = Decode(textToDecode, charset, encoding);
            }

            return decodedText;
        }

        public bool IsValidEncodingFormat(string name)
        {
            return (name.Contains(CharsetPrefix) && name.Contains(EncodingPerfix) && name.Contains(EndcodedTextSuffix));
        }

        /// That method has no usage here.
        public string Encode(string decodedText, string charset, string encoding)
        {
            throw new NotImplementedException();
        }
    }
}