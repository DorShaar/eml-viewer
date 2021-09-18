using System.IO;

namespace Parser
{
    public interface ISaveable
    {
        void Save(string outputFilePath);
        void Save(TextWriter textWriter);
    }
}