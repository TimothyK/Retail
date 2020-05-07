using System.IO;

namespace Retail.Data.Tests.Helpers
{
    internal static class FileExtensions
    {
        public static FileInfo Combine(this DirectoryInfo folder, string path)
        {
            return new FileInfo(Path.Combine(folder.FullName, path));
        }
    }
}
