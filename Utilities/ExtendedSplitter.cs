using System.Collections.Generic;

namespace Utilities
{
    public class ExtendedSplitter
    {
        /// <summary>
        /// Splits <param name="strToSplit"></param> by <param name="separetorsTokens"></param>.
        /// Example: "?ab?=c", {?, ?=} will be splitted into "", "ab" and "c".
        /// </summary>
        public static List<string> Split(string strToSplit, string[] separetorsTokens)
        {
            List<string> splittedStrings = new List<string>();

            string subString = strToSplit;
            string splittedString = string.Empty;
            int separetorIndex;
            foreach (string separetor in separetorsTokens)
            {
                separetorIndex = subString.IndexOf(separetor);
                if (separetorIndex == -1)
                {
                    break;
                }

                splittedString = subString.Substring(0, separetorIndex);
                splittedStrings.Add(splittedString);
                subString = subString.Substring(separetor.Length + splittedString.Length);
            }

            splittedStrings.Add(subString);
            return splittedStrings;
        }

        /// <summary>
        /// Splits <param name="strToSplit"></param> to lines in max length of <param name="length"></param>.
        /// Does not break words.
        /// </summary>
        public static List<string> SplitByLength(string strToSplit, int length)
        {
            List<string> splittedStrings = new List<string>();

            if (strToSplit != string.Empty)
            {
                if (strToSplit.Length <= length)
                    splittedStrings.Add(strToSplit);
                else
                {
                    int firstSpaceIndex = length;
                    for (int i = length; i >= 0; --i)
                    {
                        if (char.IsWhiteSpace(strToSplit[i]))
                        {
                            firstSpaceIndex = i;
                            break;
                        }
                    }

                    // No white space found.
                    if (firstSpaceIndex == length)
                    {
                        firstSpaceIndex = strToSplit.FirstIndexOfAnySpace();
                        if (firstSpaceIndex == -1)
                        {
                            firstSpaceIndex = strToSplit.Length;
                        }
                    }

                    // Spliting string to left and right.
                    string leftSubString = strToSplit.Substring(0, firstSpaceIndex);
                    string rightSubString = string.Empty;
                    if (firstSpaceIndex < strToSplit.Length)
                        rightSubString = strToSplit.Substring(firstSpaceIndex + 1);

                    /// In case no space is found and larger than <param name="length"></param>.
                    if (firstSpaceIndex == strToSplit.Length)
                        splittedStrings.AddRange(SplitByLength(leftSubString, strToSplit.Length));
                    // In regular case.
                    else
                        splittedStrings.AddRange(SplitByLength(leftSubString, length));

                    splittedStrings.AddRange(SplitByLength(rightSubString, length));
                }
            }

            return splittedStrings;
        }
    }
}