using Utilities;
using System;
using System.Collections.Generic;

namespace EmlStructure
{
    /// <summary>
    /// RFC1341
    /// Content-Type := type "/" subtype *[";" parameter] 
    /// Note also that a subtype specification is MANDATORY. There are no default subtypes.
    /// </summary>
    [Serializable]
    public class ContentTypeInfo
    {
        public static string[] Format = new string[]
        {
            ForwardSlash,
            SemicolonSign
        };

        public const string ForwardSlash = "/";
        public const string SemicolonSign = ";";
        public const string EqualsSign = "=";

        public const string CharsetStr = "charset";

        private const string ContentTypeErrorMsg = "Unable to parse content-type";
        private const string SubtypeErrorMsg = "Unable to find sub-type";

        // Key is parameter attribute, value is parameter value.
        private Dictionary<string, string> mParameters = new Dictionary<string, string>();

        public string Type { get; private set; }
        public string SubType { get; private set; }

        public ContentTypeInfo(string contentType)
        {
            UpdateContentTypeInfo(contentType);
        }

        public string ContentTypeName
        {
            get { return $"{Type}/{SubType}"; }
            set { /* tODO parse value into type and subtype */ }
        }

        public string Charset
        {
            get { return GetParameterValue(CharsetStr); }
        }

        public string GetParameterValue(string parameterAttribute)
        {
            mParameters.TryGetValue(parameterAttribute, out string parameterValue);

            if (parameterValue == null)
                parameterValue = string.Empty;

            return parameterValue;
        }

        /// <summary>
        /// Gets the value of the content type (Without <see cref="EmlVariables.HeaderVariables.ContentTypeStr"/>.
        /// Extract type, subtype, and parameter's attributes and values. 
        /// </summary>
        /// <param name="contentType"></param>
        private void UpdateContentTypeInfo(string contentType)
        {
            // Extracts type and subtype.
            List<string> contentTypeParts = ExtendedSplitter.Split(contentType, Format);

            // In case the FORWARD_SLASH was not found - invalid.
            if (contentTypeParts.Count <= 1)
                throw new FormatException(ContentTypeErrorMsg);

            // Updating Type.
            Type = contentTypeParts[0].RemoveWhitespaces();

            // Updating SubType.
            SubType = contentTypeParts[1];

            // No ';' was found. If seeing white space, we let it pass altough
            // that is not according to RFC1341 (no spaces allowed).
            if (contentTypeParts.Count == 2)
                return;

            // Updating parameters.
            // Parameter is not obligatory. Should not throw exception here.
            if (string.IsNullOrWhiteSpace(contentTypeParts[2]))
                return;

            string[] splittedContentType = contentTypeParts[2].Split(';');
            foreach (string parameters in splittedContentType)
            {
                // Should be array of two strings - attribute and it's value only.
                List<string> attributeAndValue = ExtendedSplitter.Split(
                    parameters,
                    new string[] { EqualsSign });

                mParameters.Add(
                    attributeAndValue[0].RemoveWhitespaces(),
                    attributeAndValue[1].RemoveWhitespaces().Replace(EmlVariables.QuotationMark, string.Empty));
            }
        }
    }
}
