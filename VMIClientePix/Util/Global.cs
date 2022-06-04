using System;
using System.IO;

namespace VMIClientePix.Util
{
    public static class Global
    {
        public static readonly string AppDocumentsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VMIClientePix");
    }
}
