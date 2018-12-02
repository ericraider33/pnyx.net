using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace pnyx.net.test.util
{
    public static class TestUtil
    {
        public const String ENV_TEST_FILES = "PNYX_TEST_FILES";
        public const String ENV_TEST_OUTPUT = "PNYX_TEST_OUTPUT";
        
        public static String findTestFileLocation()
        {
            String path = Environment.GetEnvironmentVariable(ENV_TEST_FILES);
            if (path == null)
            {
                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                while (!path.EndsWith("pnyx.net.test"))
                {
                    DirectoryInfo parent = Directory.GetParent(path);
                    if (parent == null)
                        throw new IOException(String.Format("Could not find location to pnyx.net.test. Set '{0}' EVN to fix", ENV_TEST_FILES));
                    
                    path = parent.FullName;
                }

                path = Path.Combine(path, "files");
            }

            if (!Directory.Exists(path))
                throw new IOException(String.Format("Could not find location to test files: " + path));

            return path;
        }
        
        public static String findTestOutputLocation()
        {
            String path = Environment.GetEnvironmentVariable(ENV_TEST_OUTPUT);
            if (path == null)
            {
                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                while (!path.EndsWith("pnyx.net.test"))
                {
                    DirectoryInfo parent = Directory.GetParent(path);
                    if (parent == null)
                        throw new IOException(String.Format("Could not find location to pnyx.net.test. Set '{0}' EVN to fix", ENV_TEST_OUTPUT));
                    
                    path = parent.FullName;
                }

                path = Path.Combine(path, "out");

                DirectoryInfo info = new DirectoryInfo(path);
                if (!info.Exists)
                    info.Create();
            }

            if (!Directory.Exists(path))
                throw new IOException(String.Format("Could not find location to test files: " + path));

            return path;
        }

        public static String binaryDiff(String source, String dest)
        {
            FileStream sourceStream = null, destStream = null;

            try
            {
                sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read);
                destStream = new FileStream(dest, FileMode.Open, FileAccess.Read);

                int position = 0;
                int current;
                while ((current = sourceStream.ReadByte()) != -1)
                {
                    int compare = destStream.ReadByte();
                    if (current != compare)
                        return String.Format("Byte at position 0x{0:x2} is different 0x{1:x2} != 0x{2:x2} / {3} != {4}", position, current, compare, (char)current, (char)Math.Max(0,compare));

                    position++;
                }

                if (destStream.ReadByte() != -1)
                    return "Destination file has more data then source file";

                return null;
            }
            finally 
            {
                try { if (sourceStream != null) sourceStream.Dispose(); } catch (Exception) { /*ignore*/ }
                try { if (destStream != null) destStream.Dispose(); } catch (Exception) { /*ignore*/ }
            }
        }

        public static void assertArrayEquals<T>(T[] a, T[] b)
        {
            String aText = String.Join(",", a);
            String bText = String.Join(",", b);
            Assert.Equal(aText, bText);
        }
    }
}