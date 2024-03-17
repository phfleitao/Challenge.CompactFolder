using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CompactFolder.Cli.Tests.Integration.TestUtils.Helpers
{
    public static class FileHelper
    {
        public static void ClearFolderSkippingProcessingFiles(string path)
        {
            foreach (var filePath in Directory.GetFiles(path))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (IOException)
                {
                    // File is in use by another process, skip it
                    continue;
                }
            }

            foreach (var subfolderPath in Directory.GetDirectories(path))
            {
                try
                {
                    Directory.Delete(subfolderPath, true);
                }
                catch (IOException)
                {
                    // Directory is in use by another process, skip it
                    continue;
                }
            }
        }

        public static IEnumerable<string> GetFilesInDirectory(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        }

        public static IEnumerable<string> GetRelativeFilesInDirectory(string directoryPath)
        {
            var basePathToRemove = directoryPath + "\\";

            var files = GetFilesInDirectory(directoryPath)
                        .Select(f => f.Remove(0, basePathToRemove.Length));
            return files;
        }

        public static IEnumerable<string> GetFoldersInDirectory(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories);
        }

        public static IEnumerable<string> GetRelativeFoldersInDirectory(string directoryPath)
        {
            var basePathToRemove = directoryPath + "\\";

            var folders = GetFoldersInDirectory(directoryPath)
                        .Select(f => f.Remove(0, basePathToRemove.Length));
            return folders;
        }

        public static IEnumerable<string> GetDistinctFileNamesWithoutExtension(string directoryPath)
        {
            var filenamesWithoutExtension = GetFilesInDirectory(directoryPath)
                .Select(file => Path.GetFileNameWithoutExtension(file))
                .Where(filename => !string.IsNullOrWhiteSpace(filename))
                .Distinct();

            return filenamesWithoutExtension;
        }

        public static IEnumerable<string> GetDistinctDirectoryNames(string directoryPath)
        {
            var directoryNames = GetFoldersInDirectory(directoryPath)
                .Select(directory => Path.GetFileName(directory))
                .Where(directoryName => !string.IsNullOrEmpty(directoryName))
                .Distinct();

            return directoryNames;
        }

        public static IEnumerable<string> GetDistinctFileExtensions(string directoryPath)
        {
            var extensions = GetFilesInDirectory(directoryPath)
                .Select(file => Path.GetExtension(file)?.ToLower())
                .Where(extension => !string.IsNullOrWhiteSpace(extension))
                .Distinct();

            return extensions;
        }

        public static bool DoesAnyFileExtensionsExistInPath(string directoryPath, IEnumerable<string> excludedExtensions)
        {
            var filesInDirectory = GetFilesInDirectory(directoryPath);

            return filesInDirectory.Any(file =>
                excludedExtensions.Any(excludedExtension =>
                    string.Equals(Path.GetExtension(file), excludedExtension, StringComparison.OrdinalIgnoreCase)));
        }

        public static bool DoesAnyFileNameExistInPath(string directoryPath, IEnumerable<string> excludedFileNames)
        {
            var filesInDirectory = GetDistinctFileNamesWithoutExtension(directoryPath);

            return filesInDirectory.Any(fileName =>
                excludedFileNames.Any(excludedFileName =>
                    string.Equals(fileName, excludedFileName, StringComparison.OrdinalIgnoreCase)));
        }

        public static bool DoesAnyDirectoryExistInPath(string directoryPath, IEnumerable<string> excludedDirectoryNames)
        {
            var directoriesInBaseFolder = GetDistinctDirectoryNames(directoryPath);

            return directoriesInBaseFolder.Any(directoryName =>
                excludedDirectoryNames.Any(excludedDirectoryName =>
                    string.Equals(directoryName, excludedDirectoryName, StringComparison.OrdinalIgnoreCase)));
        }

        public static bool IsFolderEquals(string leftFolderPath, string rightFolderPath)
        {
            var leftInfo = GetRelativeFilesInDirectory(leftFolderPath)
                            .Union(GetRelativeFoldersInDirectory(leftFolderPath))
                            .OrderBy(name => name);
            var rightInfo = GetRelativeFilesInDirectory(rightFolderPath)
                            .Union(GetRelativeFoldersInDirectory(rightFolderPath))
                            .OrderBy(name => name);
            return leftInfo.SequenceEqual(rightInfo);
        }

        public static IEnumerable<T> SelectRandomItems<T>(IEnumerable<T> list, int quantity)
        {
            var shuffledList = list.OrderBy(x => Guid.NewGuid());
            return shuffledList.Take(quantity).ToList();
        }

        public static void UnzipFolder(string zipFilePath, string extractFolderPath)
        {
            ZipFile.ExtractToDirectory(zipFilePath, extractFolderPath);
        }
    }
}
