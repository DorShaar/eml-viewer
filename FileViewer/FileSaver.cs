using Parser;
using System.IO;

namespace FileViewer
{
    internal class FileSaver
    {
        /// <summary>
        /// Returns the directory of the saved file.
        /// </summary>
        public string Save(IParsable fileToSave, string savedFilePath)
        {
            fileToSave.Save(savedFilePath);
            return Path.GetDirectoryName(savedFilePath);
        }

        public string Save(IParsable fileToSave)
        {
            fileToSave.Save(fileToSave.FilePath);
            return Path.GetDirectoryName(fileToSave.FilePath);
        }
    }
}
