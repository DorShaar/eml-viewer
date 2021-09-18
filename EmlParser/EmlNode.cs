using Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmlStructure
{
    /// <summary>
    /// EmlNode contains two parts - Headers and Body.
    /// </summary>
    [Serializable]
    public class EmlNode : IEmlNode
    {
        // Indicates if that is the first node in the tree.
        // Helps Save() method indicates if should override existing file.
        private readonly bool mIsRootNode = false;
        public IList<ITreeNode> ChildNodes { get; }
        public EmlHeader Header { get; }

        public IEnumerable KeyValuePair
        {
            get { return Header; }
        }

        public string Name
        {
            get { return GetNodeName(); }
            set { Header.ContentType.ContentTypeName = value; }
        }

        public string FilePath { get; set; } = null;

        public EmlNode(EmlHeader emlHeader, IList<ITreeNode> body, bool isRootNode)
        {
            Header = emlHeader;
            ChildNodes = body;

            mIsRootNode = isRootNode;
        }

        public string GetBoundary()
        {
            return Header.Boundary;
        }

        // Save operation is appending since different nodes approach to the same file
        // and adding data, so we should check if the file is existing before. + COMEMENT THAT.
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

        public void Save(TextWriter textWriter)
        {
            // Writes header.
            Header.Save(textWriter);

            // Writes body.
            foreach (IEmlNode subEml in ChildNodes)
            {
                textWriter.WriteLine(Header.Boundary);
                subEml.Save(textWriter);
            }

            // Writes closing sub-eml (boundary and header-body line seperator).
            textWriter.WriteLine(Header.FullBoundary);
        }

        IParsable IParsable.Clone()
        {
            Cloner cloner = new Cloner();
            return cloner.DeepClone(this);
        }

        private void DeleteExistingEml(string emlFilePath)
        {
            File.Delete(emlFilePath);
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

        // TODO think about encoding.
        private Encoding GetEncodingType()
        {
            return Encoding.UTF8;
        }
    }
}
