using Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities;

namespace EmlStructure
{
    public class EmlParser : IParser<string>
    {
        private static int mNodeDepth = 0;

        // Data collected from first read.
        private const int FirstLinesNumberToRead = 30;
        private Encoding mEncoding = Encoding.UTF8;

        /// <summary>
        /// Gets file path, reads it's data and parses it into header and body. 
        /// </summary>
        ///
        public IParsable Parse(string fileToParse)
        {
            CollectDataByInitialScan(fileToParse);
            return InnerParse(File.ReadAllText(fileToParse, mEncoding));
        }

        private void CollectDataByInitialScan(string filePath)
        {
            // This is for reading the first x lines, taking the charset and set the encoding.
            List<string> first10Lines = File.ReadLines(filePath)
                                            .Take(FirstLinesNumberToRead)
                                            .ToList();

            string contentTypeLine = first10Lines.Find(line => line.Contains(EmlHeader.ContentTypeStr));
            if(!string.IsNullOrEmpty(contentTypeLine))
            {
                ContentTypeInfo contentTypeInfo = new ContentTypeInfo(contentTypeLine);
                string charset = contentTypeInfo.Charset;
                if (!string.IsNullOrEmpty(charset))
                {
                    mEncoding = Encoding.GetEncoding(charset);
                }
            }
        }

        private IParsable InnerParse(string data)
        {
            IParsable emlNode;

            // Collecting Head and Body data.
            CollectNodeHeaderAndBody(
                data,
                out string collectedHeaders,
                out string collectedBody);

            // Parsing data.
            EmlHeader header = ParseHeader(collectedHeaders);
            if (header.IsContentTypeMultipart())
            {
                mNodeDepth++;
                List<ITreeNode> body = ParseMultipartBody(collectedBody, header.Boundary);
                mNodeDepth--;
                emlNode = new EmlNode(header, body, IsParsingRootNode());
            }
            else if(header.IsAttachedEml())
            {
                mNodeDepth++;
                List<ITreeNode> body = new List<ITreeNode>
                {
                    InnerParse(collectedBody) as ITreeNode
                };

                mNodeDepth--;
                emlNode = new EmlNode(header, body, IsParsingRootNode());
            }
            else
                emlNode = new EmlLeafNode(header, collectedBody, IsParsingRootNode());

            return emlNode;
        }

        /// <summary>
        /// Reads headers.
        /// If line starts with a space, it means that that line is a continue for the previous.
        /// Saving all header values with space at the begining.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private EmlHeader ParseHeader(string data)
        {
            return new EmlHeader(ExtractHeadersKeysValues(data));
        }

        internal Dictionary<string, List<string>> ExtractHeadersKeysValues(string data)
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();
            string headerName = string.Empty;
            StringBuilder headerValue = new StringBuilder();

            string[] dataAsLines = data.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in dataAsLines)
            {
                // Line that is a continious from the former line.
                if (IsContiniousLine(line))
                    headerValue.Append(line);

                // New header.
                else
                {
                    // Adding previous header.
                    AddHeader(headers, headerName, headerValue.ToString());
                    headerName = string.Empty;

                    // Handling new header.
                    int colonIndex = line.IndexOf(EmlVariables.Colon);

                    // That is garbage text, not an header.
                    if (colonIndex == -1)
                        continue;
                    else
                    {
                        headerName = line.Substring(0, colonIndex);
                        headerValue = new StringBuilder();

                        // +1 for zero based index.
                        // We continue to read next line in case the value is written one line below.
                        if (line.Length < colonIndex + 2)
                            continue;

                        /// +1 for zero based index and skipping and +1 for skipping space between <see cref="EmlVariables.Colon"/> and header value.
                        string extractedHeaderValue = line.Substring(colonIndex + 2);
                        headerValue.Append(extractedHeaderValue);
                    }
                }
            }

            // Adding last header.
            AddHeader(headers, headerName, headerValue.ToString());
            return headers;
        }

        private bool IsContiniousLine(string line)
        {
            return line.StartsWith(EmlVariables.WhiteSpaces.Space) ||
                   line.StartsWith(EmlVariables.WhiteSpaces.HorizontalTab) ||
                   line.StartsWith(EmlVariables.WhiteSpaces.VerticalTab);
        }

        private void AddHeader(Dictionary<string, List<string>> headers, string headerName, string headerValue)
        {
            if (!string.IsNullOrEmpty(headerName))
            {
                if (headers.TryGetValue(headerName, out List<string> values))
                    values.Add(headerValue);
                else
                    headers.Add(headerName, new List<string> { headerValue });
            }
        }

        /// <summary>
        /// Identifies Header and Body:
        /// In case the EML has Content-Type "multipart" - the start of the body is found right after
        /// the given boundary.
        /// On other cases, the start of the body is found after two new lines together.
        /// Header will contain the header-body separetor.
        /// </summary>
        private void CollectNodeHeaderAndBody(string data, out string headers, out string body)
        {
            headers = string.Empty;
            body = string.Empty;

            int twoNewLinesFirstIndex = data.FirstIndexOfTwoNewLines(out string twoNewLines);
            if (twoNewLinesFirstIndex == -1)
                throw new FormatException("Cannot identify headers and body");

            int bodyStartIndex = twoNewLinesFirstIndex + twoNewLines.Length;
            headers = data.Substring(0, bodyStartIndex);
            EmlHeader header = ParseHeader(headers);
            if (header.IsContentTypeMultipart())
            {
                int boundaryStartIndex = data.IndexOf(header.Boundary);
                headers = data.Substring(0, boundaryStartIndex);
            }

            body = data.Substring(bodyStartIndex);
        }

        /// <summary>
        /// Gets body content including the boundary token. Extracting all the sub bodies (all the nodes) from that content, and
        /// parse them.
        /// </summary>
        /// <param name="bodyContent"></param>
        /// <param name="boundaryToken"></param>
        /// <returns></returns>
        private List<ITreeNode> ParseMultipartBody(string bodyContent, string boundaryToken)
        {
            // Preparing to populate Body.
            List<ITreeNode> body = new List<ITreeNode>();

            bool isFirstSubstringExtract = true;
            int startBoundaryIndex = 0;
            string subBody;
            foreach (int nextBoundaryIndex in bodyContent.AllIndexesOf(boundaryToken))
            {
                // First body.Substring(0, <firstIndex>) will not generate valid subEml.
                if (isFirstSubstringExtract)
                {
                    isFirstSubstringExtract = false;
                    startBoundaryIndex = nextBoundaryIndex;
                    continue;
                }

                int length = nextBoundaryIndex - startBoundaryIndex;
                subBody = bodyContent.Substring(
                    startBoundaryIndex + boundaryToken.Length,
                    length);

                subBody = subBody.Replace(boundaryToken, string.Empty);

                startBoundaryIndex = nextBoundaryIndex;

                IEmlNode emlNode = InnerParse(subBody) as IEmlNode;
                body.Add(emlNode);
            }

            return body;
        }

        /// <summary>
        /// Should create Eml with signing if the current node is root or not.
        /// </summary>
        /// <returns></returns>
        private bool IsParsingRootNode()
        {
            return mNodeDepth == 0;
        }
    }
}
