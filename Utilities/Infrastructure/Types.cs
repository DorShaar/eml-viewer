using System.Collections.Generic;

namespace Utilities
{
    public class Types
    {
        private const string UnkownType = "Unkown";
        public static readonly string Eml = ".eml";

        // Map between extension and type.
        private static Dictionary<string, string> mRegisteredTypes = new Dictionary<string, string>
        {
            { Eml, "eml" }
        };

        public static string GetType(string extension)
        {
            if (!mRegisteredTypes.TryGetValue(extension, out string type))
            {
                type = UnkownType;
            }

            return extension;
        }

        public bool RegisterNewType(string newTypeExtension, string newType)
        {
            bool isRegisterSucceed = true;

            if(mRegisteredTypes.ContainsKey(newTypeExtension))
            {
                isRegisterSucceed = false;
            }
            else
            {
                mRegisteredTypes.Add(newTypeExtension, newType);
            }

            return isRegisterSucceed;
        }
    }
}
