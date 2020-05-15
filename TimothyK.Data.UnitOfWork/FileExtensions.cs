using System.IO;

namespace TimothyK.Data.UnitOfWork
{
    internal static class FileExtensions
    {
        public static FileInfo Combine(this DirectoryInfo folder, string path)
        {
            return new FileInfo(Path.Combine(folder.FullName, path));
        }
    }
}
