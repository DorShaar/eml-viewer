using System.Collections;
using System.Collections.Generic;

namespace Parser
{
    public interface ITreeNode : IParsable
    {
        IList<ITreeNode> ChildNodes { get; }
        string Name { get; set; }
        IEnumerable KeyValuePair { get; }
    }

    public interface ILeafNode
    {
        string Name { get; set; }
        string Text { get; set; }
    }
}
