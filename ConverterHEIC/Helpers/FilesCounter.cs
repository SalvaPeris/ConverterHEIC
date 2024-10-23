using System.IO;

namespace ConverterHEIC.Helpers
{
    internal class FilesCounter
    {
        public static string[] Counter(string directoryPath, string searchPattern)
        {
            string[] heicFiles = Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
            return heicFiles;
        }
    }
}
