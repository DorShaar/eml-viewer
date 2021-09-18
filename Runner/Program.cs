using EmlStructure;
using Parser;
using System;
using System.IO;
using System.Text;

namespace Runner
{
    // TODO FIx textplain8bit text cannot be decoded
    // TODO make large text enumrable.
    // TODO add eml Node
    // TODO make headers modifiedable.

    class Program
    {
        private static IParser<string> emlParser = new EmlParser();

        static void Main(string[] args)
        {
            if (args[0] == null)
                return;

            EmlDecoderEncoder emlDecoderEncoder = new EmlDecoderEncoder();
            RunOnSingle(args[0]);
        }

        private static void RunOnSingle(string fileToUse)
        {
            // Read and parse.
            string data = File.ReadAllText(fileToUse, Encoding.Default);
            IEmlNode emlData = emlParser.Parse(data) as IEmlNode;

            //emlData.Save(Path.Combine(outputAttachmentDirectory, "resaved.eml"));
            //Extract(emlData, outputAttachmentDirectory);
        }

        private static void RunAllOver(string directoryToUse, string outputAttachmentDirectory)
        {
            foreach (string filePath in Directory.GetFiles(directoryToUse))
            {
                Console.WriteLine($"Reading {filePath}");
                string data = File.ReadAllText(filePath, Encoding.Default);

                IEmlNode emlData = emlParser.Parse(data) as IEmlNode;

                Extract(emlData, outputAttachmentDirectory);
            }
        }

        private static void Extract(IEmlNode emlData, string outputAttachmentDirectory)
        {
            if (emlData is EmlNode)
            {
                foreach (IEmlNode emlNode in (emlData as EmlNode).ChildNodes)
                {
                    (new EmlExtractor()).Extract(emlNode, " ");
                }
            }
        }
    }
}
