using System;
using System.IO;

namespace CompactFolder.Cli.Tests.Integration.Helpers
{
    public class RandomFileSystemGeneratorHelper
    {
        private static Random random = new Random();
        private static string[] extensions = new string[]
        {
            ".txt"
            ,".jpg"
            ,".bmp"
            ,".pdf"
            ,".doc"
            ,".docx"
            ,".xls"
            ,".xlsx"
            ,".xml"
            ,".png"
            ,".json"
        };

        public static void GenerateRandomFoldersAndFiles(string rootDirectory, int depth, int maxFoldersPerDepth, int maxFilesPerFolder)
        {
            GenerateDirectory(rootDirectory, depth, maxFoldersPerDepth, maxFilesPerFolder);
        }

        private static void GenerateDirectory(string currentDirectory, int remainingDepth, int maxFolders, int maxFiles)
        {
            if (remainingDepth <= 0)
                return;

            Directory.CreateDirectory(currentDirectory);
            CreateFilesInDisk(currentDirectory, maxFiles);
            CreateFoldersInDisk(currentDirectory, remainingDepth, maxFolders, maxFiles);
        }

        private static void CreateFoldersInDisk(string currentDirectory, int remainingDepth, int maxFolders, int maxFiles)
        {
            int subDirectoryCount = GenerateRandomNumberForFilesAndFolders(maxFolders);
            for (int i = 0; i < subDirectoryCount; i++)
            {
                string subDirectory = Path.Combine(currentDirectory, $"SubDirectory_{remainingDepth}{i}");
                GenerateDirectory(subDirectory, remainingDepth - 1, maxFolders, maxFiles);
            }
        }

        private static void CreateFilesInDisk(string currentDirectory, int maxFiles)
        {
            int fileCount = GenerateRandomNumberForFilesAndFolders(maxFiles);
            for (int i = 0; i < fileCount; i++)
            {
                string filePath;

                if (GenerateUniqueFile())
                {
                    filePath = Path.Combine(currentDirectory, 
                        $"UniqueFile{Guid.NewGuid().ToString()}{GenerateRandomExtension()}");
                }
                else
                {
                    filePath = Path.Combine(currentDirectory, 
                        $"File{i}{GenerateRandomExtension()}");
                }
                
                File.WriteAllText(filePath, $"Content of File{i}");
            }
        }

        private static int GenerateRandomNumberForFilesAndFolders(int maxValue)
        {
            return random.Next(maxValue) + 1;
        }

        private static string GenerateRandomExtension()
        {
            int randomIndex = random.Next(extensions.Length);
            return extensions[randomIndex];
        }

        private static bool GenerateUniqueFile()
        {
            const int UNIQUE_FILE_VALUE = 1;

            var randomUniqueValue = random.Next(2);
            if (randomUniqueValue == UNIQUE_FILE_VALUE)
                return true;
            return false;
        }
    }
}
