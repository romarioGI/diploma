using System.IO;

namespace InputSubsystem
{
    public class FileInput : TextReaderInput
    {
        private static TextReader GetFileTextReader(string fileName)
        {
            return new StreamReader(fileName);
        }

        public FileInput(string fileName) : base(GetFileTextReader(fileName))
        {
        }
    }
}