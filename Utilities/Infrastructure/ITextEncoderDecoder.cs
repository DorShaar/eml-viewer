
namespace Utilities
{
    public interface ITextEncoderDecoder
    {
        string Decode(object itemData);
        string Decode(string encodedText, string charset, string encoding);
        string Encode(string decodedText, string charset, string encoding);
    }
}
