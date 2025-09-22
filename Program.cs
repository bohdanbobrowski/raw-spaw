using System;
using System.IO;
using System.Collections;

class Program
{
    static List<string> GetListOfRawFiles()
    {
        List<string> rawFiles = new List<string>();
        string path = Directory.GetCurrentDirectory();
        Console.WriteLine("Listing files in {0} folder", path);
        string[] fileEntries = Directory.GetFiles(".");
        foreach (string fileName in fileEntries)
        {
            if (fileName.EndsWith(".DNG"))
                rawFiles.Add(fileName);
        }
        return rawFiles;
    }

    static void MoveRawFile(string rawFile)
    {
        string rawFileName = Path.GetFileNameWithoutExtension(rawFile);
        var dirName = new DirectoryInfo(".").Name;
        string destinationPath = Path.Combine(dirName, rawFileName + ".DNG");
        if (!Directory.Exists("raw"))
        {
            // Directory.CreateDirectory("raw");
            Console.WriteLine("Create directory: {0}", dirName);
        }
        // File.Move(rawFile, destinationPath);
        Console.WriteLine("Move {0} to {1}", rawFile, destinationPath);
    }

    static void Main(string[] args)
    {
        List<string> rawFiles = GetListOfRawFiles();
        foreach (string rawFile in rawFiles)
        {
            bool moveRaw = false;
            string rawFileName = Path.GetFileNameWithoutExtension(rawFile);
            if (!File.Exists(rawFileName + ".JPG"))
            {
                moveRaw = true;
            } else
            {
                // TODO: read exif
                Console.WriteLine("{0}.JPG exists", rawFileName);
            }
            if (moveRaw)
            {
                MoveRawFile(rawFile);
            }
        }
    }
}