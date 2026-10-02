using System.IO;

namespace PhotoConverter.Helpers
{
    internal class FilesCounter
    {
        public static string[] Counter(string directoryPath, params string[] searchPatterns)
        {
            return searchPatterns
                .SelectMany(pattern =>
                    Directory.GetFiles(directoryPath, pattern, SearchOption.AllDirectories))
                .ToArray();
        }
    }
}
