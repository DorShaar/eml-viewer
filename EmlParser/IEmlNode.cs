using Parser;

namespace EmlStructure
{
    public interface IEmlNode : ITreeNode
    {
        EmlHeader Header { get; }
    }
}
