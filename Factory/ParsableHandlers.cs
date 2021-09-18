using Parser;
using Utilities;

namespace Factory
{
    public class ParsableHandlers
    {
        public IDataExtractor<object> DataExtractor { get; }
        public ITextEncoderDecoder TextEncoderDecoder { get;  }
        public IParser<string> Parser { get; }

        public ParsableHandlers(IDataExtractor<object> dataExtractor, ITextEncoderDecoder textEncoderDecoder, IParser<string> parser)
        {
            DataExtractor = dataExtractor;
            TextEncoderDecoder = textEncoderDecoder;
            Parser = parser;
        }
    }
}
