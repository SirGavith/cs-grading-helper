using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide the path to the ZIP file.");
            return;
        }

        string zipFilePath = args[0];

        if (!File.Exists(zipFilePath))
        {
            Console.WriteLine("The specified file does not exist.");
            return;
        }

        string destinationDirectory = Path.Combine(
            Directory.GetParent(zipFilePath)?.FullName ?? ".",
            Path.GetFileNameWithoutExtension(zipFilePath)
        );

        try
        {
            UnzipProjects(zipFilePath, destinationDirectory);
            Console.WriteLine($"All projects extracted to {destinationDirectory}");

            OpenProjectsInVSCode(destinationDirectory);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void UnzipProjects(string zipFilePath, string destinationDirectory)
    {
        ZipFile.ExtractToDirectory(zipFilePath, destinationDirectory);
        Console.WriteLine($"Extracted {zipFilePath} to {destinationDirectory}");

        foreach (string file in Directory.GetFiles(destinationDirectory, "*.zip", SearchOption.AllDirectories))
        {
            ZipFile.ExtractToDirectory(file, destinationDirectory);
            File.Delete(file);
            Console.WriteLine($"Extracted {Path.GetFileName(file)}");
        }
    }

    static void OpenProjectsInVSCode(string destinationDirectory)
    {
        var projects = Directory.GetDirectories(destinationDirectory);

        foreach (var folder in projects)
        {
            Console.WriteLine($"\nWould you like to open the following folder in VS Code? {folder} (y/n)");
            var response = Console.ReadLine();

            if (response?.Trim().ToLower() == "y")
            {
                OpenInVSCode(folder);
            }
        }
    }

    static void OpenInVSCode(string folderPath)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "code",
            Arguments = $"\"{folderPath}\"",
            UseShellExecute = true
        });
        Console.WriteLine($"Opening {folderPath} in VS Code...");
    }
}
