using System;

namespace TranslateMe.FileHandling
{
    public class FileLoadException : Exception
    {
        public FileLoadException(string message)
            : base(message)
        {
        }

        public FileLoadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}