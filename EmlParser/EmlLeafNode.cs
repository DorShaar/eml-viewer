using Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmlStructure
{
    /// <summary>
    /// That is the leaf in the tree. Not multipart.
    /// </summary>
    [Serializable]
    public class EmlLeafNode : IEmlNode, ILeafNode
    {
        // Indicates if that is the first node in the tree.
        // Helps Save() method indicates if should override existing file.
        private readonly bool mIsRootNode = false;

        // ChildNodes Should be empty in EmlLeafNode.
        public IList<ITreeNode> ChildNodes { get; set; } = new List<ITreeNode>();
        public EmlHeader Header { get; private set; }
        public IEnumerable KeyValuePair
        {
            get { return Header; }
        }
        public string Body { get; private set; }
        public string Name
        {
            get { return GetNodeName(); }
            set { Header.ContentType.ContentTypeName = value; }
        }

        public string Text
        {
            get { return Body; }
            set { Body = value; }
        }

        public string FilePath { get; set; } = null;

        public EmlLeafNode(EmlHeader emlHeader, string body, bool isRootNode)
        {
            Header = emlHeader;
            Body = body;
            mIsRootNode = isRootNode;
        }

        public void Save(string resavedFilePath)
        {
            if (mIsRootNode)
            {
                DeleteExistingEml(resavedFilePath);
            }

            using (TextWriter textWriter = new StreamWriter(resavedFilePath, append: true, encoding: GetEncodingType()))
            {
                Save(textWriter);
            }
        }

        /// <summary>
        /// Must ensure that the body contains new line at the end.
        /// </summary>
        /// <param name="textWriter"></param>
        public void Save(TextWriter textWriter)
        {
            Header.Save(textWriter);

            // Validates that Body contains new line at the end.
            if(!Body.EndsWith(Environment.NewLine))
            {
                Body += Environment.NewLine;
            }

            textWriter.Write(Body);
        }

        private string GetNodeName()
        {
            string nodeName = Header.ContentType.ContentTypeName;
            string description = string.Empty;

            if (Header.IsAttachment())
                description = "attachment";

            if (!string.IsNullOrEmpty(description))
                nodeName = $"[{description}] {nodeName}";

            return nodeName;
        }

        private void DeleteExistingEml(string emlFilePath)
        {
            File.Delete(emlFilePath);
        }

        // TODO think about encoding.
        private Encoding GetEncodingType()
        {
            return Encoding.UTF8;
        }

        IParsable IParsable.Clone()
        {
            Cloner cloner = new Cloner();
            return cloner.DeepClone(this);
        }
    }
}
