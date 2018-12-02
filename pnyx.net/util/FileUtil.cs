using System;
using System.Collections.Generic;
using System.IO;

namespace pnyx.net.util
{
    public static class FileUtil
    {        
        public static void assureDirectoryStructExists(String absolutePath)
        {                        
            List<DirectoryInfo> toBuild = new List<DirectoryInfo>();            
            DirectoryInfo parent = Directory.GetParent(absolutePath);
            while (parent != null && !parent.Exists)
            {
                toBuild.Insert(0, parent);
                parent = parent.Parent;
            }

            foreach (DirectoryInfo di in toBuild)
                di.Create();
        }
    }
}