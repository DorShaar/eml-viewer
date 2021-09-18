
namespace Utilities
{
    public interface IDataExtractor<T>
    {
        string GetName(T itemData);
        void Extract(T itemData, string filePath);
    }
}
