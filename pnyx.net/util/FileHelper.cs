using System;
using System.IO;

namespace pnyx.net.util
{
    public static class FileHelper
    {        
        public static void assureDirectoryStructExists(String absolutePath)
        {
            String directoryPath = "" + Path.GetDirectoryName(absolutePath);
            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            if (!dir.Exists)
                dir.Create();
        }
    }
}