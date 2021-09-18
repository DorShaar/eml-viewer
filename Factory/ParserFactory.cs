using EmlStructure;
using System;
using System.Collections.Generic;
using System.IO;
using Utilities;

namespace Factory
{
    public class ParserFactory
    {
        public Dictionary<string, ParsableHandlers> FactoryMap { get; } = new Dictionary<string, ParsableHandlers>
        {
            {Types.Eml, new ParsableHandlers(new EmlExtractor(), new EmlDecoderEncoder(), new EmlParser()) }
        };

        public ParsableHandlers GetProduct(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (!FactoryMap.TryGetValue(extension, out ParsableHandlers readers))
            {
                throw new ArgumentException($"No readers are registered for extension: {extension}");
            }

            return readers;
        }
    }
}
