using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utilities;

namespace EmlStructure
{
    [Serializable]
    public class EmlHeader : IEnumerable<KeyValuePair<string, List<string>>>
    {
        // Eml header variables.
        public static readonly string MultipartStr = "multipart";
        public static readonly string BoundaryPrefixSuffix = "--";
        public static readonly string BoundaryStr = "boundary";
        public static readonly string AttachmentStr = "attachment";
        public static readonly string MessageStr = "message";

        // Eml common headers.
        public static readonly string ContentTypeStr = "Content-Type";
        public static readonly string ContentTransferEncodingStr = "Content-Transfer-Encoding";
        public static readonly string ContentDispositionStr = "Content-Disposition";
        public static readonly string ToStr = "To";

        // Headers Pair of strings which are header name and header value.
        private Dictionary<string, List<string>> mHeaders = new Dictionary<string, List<string>>();
        public ContentTypeInfo ContentType { get; private set; }


        public EmlHeader(Dictionary<string, List<string>> headers)
        {
            mHeaders = headers;
            ContentType = new ContentTypeInfo(GetContentType());
        }

        public bool IsContentTypeMultipart()
        {
            return ContentType != null &&
                   ContentType.Type.Equals(MultipartStr, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsAttachedEml()
        {
            return ContentType != null &&
                   ContentType.Type.Equals(MessageStr, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the boundary of the eml with it's prefix.
        /// </summary>
        public string Boundary
        {
            get
            {
                return BoundaryPrefixSuffix + ContentType.GetParameterValue(BoundaryStr);
            }
        }

        public string FullBoundary
        {
            get
            {
                return Boundary + BoundaryPrefixSuffix;
            }
        }

        public string ContentTransferEncoding
        {
            get
            {
                return GetHeaderValueByKey(ContentTransferEncodingStr)[0];
            }
        }

        public string ContentDisposition
        {
            get
            {
                return GetHeaderValueByKey(ContentDispositionStr)[0];
            }
        }

        private string GetContentType()
        {
            return GetHeaderValueByKey(ContentTypeStr)[0];
        }

        private List<string> GetHeaderValueByKey(string key)
        {
            if (!mHeaders.TryGetValue(key, out List<string> headerValue))
                headerValue = new List<string> { string.Empty };

            return headerValue;
        }

        /// <summary>
        /// Writes the headers. Also write header - body seperator (new line).
        /// Adds To header in case there is no one, because some headers like "To", "From" and "Subject" are required for
        /// showing the eml in outlook. Also "To", "From" and "Subject" need to be first.
        /// </summary>
        /// <param name="textWriter"></param>
        public void Save(TextWriter textWriter)
        {
            if (!mHeaders.ContainsKey(ToStr))
                AddHeader(ToStr, string.Empty);

            string headerName;
            foreach (KeyValuePair<string, List<string>> pair in this)
            {
                headerName = pair.Key;
                foreach (string headerValue in pair.Value)
                {
                    /// Keeps that single line length won't exceed <see cref="EmlVariables.MaxLineLength"/>

                    List<string> splitedHeaderValue = ExtendedSplitter.SplitByLength($"{headerName}: {headerValue}", EmlVariables.MaxLineLength);
                    StringBuilder stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine($"{splitedHeaderValue[0]}");
                    for (int i = 1; i < splitedHeaderValue.Count; ++i)
                    {
                        stringBuilder.AppendLine($" {splitedHeaderValue[i]}");
                    }

                    textWriter.Write(stringBuilder.ToString());
                }
            }

            // Writes the header - body seperator.
            textWriter.WriteLine(string.Empty);
        }

        private void AddHeader(string headerName, string headerValue)
        {
            if (!string.IsNullOrEmpty(headerName))
            {
                if (mHeaders.TryGetValue(headerName, out List<string> values))
                    values.Add(headerValue);
                else
                {
                    Dictionary<string, List<string>> newHeaders = new Dictionary<string, List<string>>
                    {
                        { headerName, new List<string> { headerValue } }
                    };

                    foreach (KeyValuePair<string, List<string>> keyValuePair in mHeaders)
                    {
                        newHeaders.Add(keyValuePair.Key, keyValuePair.Value);
                    }

                    mHeaders = newHeaders;
                }
            }
        }

        public bool IsAttachment()
        {
            return ContentDisposition.Contains(AttachmentStr);
        }

        // IEnumrable.
        public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
        {
            foreach (KeyValuePair<string, List<string>> keyValuePair in mHeaders)
            {
                yield return keyValuePair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}