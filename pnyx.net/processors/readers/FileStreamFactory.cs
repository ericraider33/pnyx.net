using System;
using System.IO;

namespace pnyx.net.api
{
    public class FileStreamFactory : IStreamFactory, IDisposable
    {
        public String path;
        public FileMode mode = FileMode.Open;
        public FileAccess access = FileAccess.Read;

        private FileStream fileStream;

        public FileStreamFactory()
        {            
        }
        
        public FileStreamFactory(String path)
        {
            this.path = path;
        }

        public Stream openStream()
        {
            if (fileStream != null)
            {
                resetStream();
                return fileStream;
            }
            
            fileStream = new FileStream(path, mode, access);
            return fileStream;
        }

        public Stream resetStream()
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            return fileStream;
        }

        public void closeStream()
        {
            fileStream.Close();
        }

        public void Dispose()
        {
            if (fileStream != null)
                fileStream.Dispose();

            fileStream = null;
        }
    }
}