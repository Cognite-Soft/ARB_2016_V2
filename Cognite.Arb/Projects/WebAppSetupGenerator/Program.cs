using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WebAppSetupGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("exe <output path> <path to folder> <feature name> <source path>");
                return;
            }
            var outputPath = args[0];
            var path = args[1];
            var featureName = args[2];
            var sourcePath = args[3];

            var result = Process(path, featureName, sourcePath);

            File.WriteAllText(outputPath, result);
        }

        private static string Process(string path, string featureName, string sourcePath)
        {
            var result = new StringBuilder();
            var directory = new DirectoryInfo(path);
            WriteDirectory(result, directory, featureName);
            result.AppendLine();
            WriteComponents(result, directory, featureName, sourcePath, string.Empty);
            return result.ToString();
        }

        private static void WriteDirectory(StringBuilder result, DirectoryInfo directory, string featureName)
        {
            result.AppendFormat("<Directory Id=\"{0}\" Name=\"{1}\">",
                GetDirectoryId(directory, featureName), directory.Name)
                .AppendLine();
            foreach (var subDirectory in directory.GetDirectories())
                WriteDirectory(result, subDirectory, featureName);
            result.AppendFormat("</Directory>").AppendLine();
        }

        private static readonly Dictionary<string, string> _directoryIdMap = new Dictionary<string, string>();

        private static string GetDirectoryId(DirectoryInfo directory, string featureName)
        {
            if (!_directoryIdMap.ContainsKey(directory.FullName))
                _directoryIdMap.Add(directory.FullName,
                    featureName + "_" + directory.Name + Guid.NewGuid().ToString().Replace("-", "") + "_FOLDER");
            return _directoryIdMap[directory.FullName];
        }

        private static void WriteComponents(StringBuilder result, DirectoryInfo directory,
            string featureName, string sourcePath, string relativePath)
        {
            result.AppendFormat("<Component Id=\"{0}\" Guid=\"{1}\" Directory=\"{2}\">",
                GetComponentId(directory, featureName), Guid.NewGuid(), GetDirectoryId(directory, featureName))
                .AppendLine()
                .AppendFormat("<CreateFolder />").AppendLine();
            foreach (var file in directory.GetFiles())
            {
                var path = Path.Combine(sourcePath, Path.Combine(relativePath, file.Name));
                result.AppendFormat("<File Id=\"{0}\" Source=\"{1}\"/>", GetNewFileId(), path).AppendLine();
            }
            result.AppendFormat("</Component>").AppendLine();

            foreach (var subDirectory in directory.GetDirectories())
                WriteComponents(result, subDirectory, featureName, sourcePath, Path.Combine(relativePath, subDirectory.Name));
        }

        private static string GetNewFileId()
        {
            return "File_" + Guid.NewGuid().ToString().Replace("-", "");
        }

        private static string GetComponentId(DirectoryInfo directory, string featureName)
        {
            return featureName + "_" + directory.Name + "_Component";
        }
    }
}