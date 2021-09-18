using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class ExtensionMethods
    {
        private const char QuotationMark = '\"';

        public static IEnumerable<int> AllIndexesOf(this string str, string subString)
        {
            if (string.IsNullOrEmpty(subString))
                throw new ArgumentException($"{subString} may not be empty");

            for (int index = 0; ; index += subString.Length)
            {
                index = str.IndexOf(subString, index);
                if (index == -1)
                    break;

                yield return index;
            }
        }

        /// <summary>
        /// Returns -1 in case no space was found.
        /// </summary>
        public static int FirstIndexOfAnySpace(this string str)
        {
            List<int> indexes = new List<int>()
            {
                str.IndexOf(GlobalVariables.HorizontalTab),
                str.IndexOf(GlobalVariables.VerticalTab),
                str.IndexOf(GlobalVariables.Space),
                str.IndexOf(GlobalVariables.WindowsNewLine),
                str.IndexOf(GlobalVariables.LinuxNewLine)
            };

            return GetMinimalNonNegativeValue(indexes);
        }

        /// <summary>
        /// Searches for two consecutive new lines, no matter if that is Windows or Linux new line.
        /// Returns -1 in case no two lines are found.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="twoNewLines"></param>
        /// <returns></returns>
        public static int FirstIndexOfTwoNewLines(this string data, out string twoNewLines)
        {
            string twoNewLinesWW = $"{GlobalVariables.WindowsNewLine}{GlobalVariables.WindowsNewLine}";
            string twoNewLinesWL = $"{GlobalVariables.WindowsNewLine}{GlobalVariables.LinuxNewLine}";
            string twoNewLinesLW = $"{GlobalVariables.LinuxNewLine}{GlobalVariables.WindowsNewLine}";
            string twoNewLinesLL = $"{GlobalVariables.LinuxNewLine}{GlobalVariables.LinuxNewLine}";

            Dictionary<int, string> indexesToNewLines = new Dictionary<int, string>();

            int indexWW = data.IndexOf(twoNewLinesWW);
            if (indexWW != -1)
            {
                indexesToNewLines.Add(indexWW, twoNewLinesWW);
            }

            int indexWL = data.IndexOf(twoNewLinesWL);
            if (indexWL != -1)
            {
                indexesToNewLines.Add(indexWL, twoNewLinesWL);
            }

            int indexLW = data.IndexOf(twoNewLinesLW);
            if (indexLW != -1)
            {
                indexesToNewLines.Add(indexLW, twoNewLinesLW);
            }

            int indexLL = data.IndexOf(twoNewLinesLL);
            if (indexLL != -1)
            {
                indexesToNewLines.Add(indexLL, twoNewLinesLL);
            }

            int minimalIndexOfTwoNewLines = GetMinimalNonNegativeValue(indexesToNewLines.Keys);
            twoNewLines = indexesToNewLines[minimalIndexOfTwoNewLines];
            return minimalIndexOfTwoNewLines;
        }

        /// <summary>
        /// Returns -1 in case no non-negative value was found.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static int GetMinimalNonNegativeValue(IEnumerable<int> values)
        {
            int minimalValue = int.MaxValue;
            foreach (int value in values)
            {
                if (value > -1 && minimalValue > value)
                    minimalValue = value;
            }

            if (minimalValue == int.MaxValue)
                minimalValue = -1;

            return minimalValue;
        }

        /// <summary>
        /// Remove whitespace from a string.
        /// If <paramref name="shouldDeleteInQuotationMarks"/> true (Default),
        /// does not remove whitespaces from string inside quotation marks.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveWhitespaces(this string str, bool shouldDeleteInQuotationMarks)
        {
            string removedWhiteSpacesStr;
            if (shouldDeleteInQuotationMarks)
            {
                removedWhiteSpacesStr =
                    new string(str.Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            }
            else
            {
                bool isInsideQuotationMarks = false;
                StringBuilder stringBuilder = new StringBuilder(str.Length);
                foreach (char ch in str)
                {
                    if (!char.IsWhiteSpace(ch))
                    {
                        stringBuilder.Append(ch);
                    }
                    else
                    {
                        if (isInsideQuotationMarks)
                        {
                            stringBuilder.Append(ch);
                        }
                    }

                    if (ch == QuotationMark)
                    {
                        isInsideQuotationMarks = !isInsideQuotationMarks;
                    }
                }

                removedWhiteSpacesStr = stringBuilder.ToString();
            }

            return removedWhiteSpacesStr;
        }

        /// <summary>
        /// Remove whitespace from a string.
        /// Does not remove whitespaces from string inside quotation marks.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveWhitespaces(this string str)
        {
            return RemoveWhitespaces(str, false);
        }

        public static string RemoveInvalidPathCharacters(this string str)
        {
            char[] InvalidCharactersInPaths = Path.GetInvalidPathChars();
            char[] InvalidCharactersInFileNames = Path.GetInvalidFileNameChars();

            string almostValidString = new string(
                str.Where(c =>
                !InvalidCharactersInPaths.Contains(c)).ToArray());
            string validString = new string(
                str.Where(c =>
                !InvalidCharactersInFileNames.Contains(c)).ToArray());

            return validString;
        }

        public static void SaveToHardDrive(this byte[] bytes, string fileFullPath)
        {
            using (Stream fileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public static string CreateNewPathWithPrefix(this string filePath, string prefix)
        {
            return Path.Combine(
                Path.GetDirectoryName(filePath),
                $"{prefix}_{Path.GetFileName(filePath)}");
        }
    }
}
