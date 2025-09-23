using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

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

    static void MoveRawFile(string rawFile, bool dryRun = false)
    {
        string rawFileName = Path.GetFileNameWithoutExtension(rawFile);
        var dirName = new DirectoryInfo(".").Name;
        string destinationPath = Path.Combine(dirName, rawFileName + ".DNG");
        if (!Directory.Exists(dirName))
        {
            if (!dryRun)
                Directory.CreateDirectory(dirName);
            Console.WriteLine("Create directory: {0}", dirName);
        }
        if (!dryRun)
            File.Move(rawFile, destinationPath);
        Console.WriteLine("{0} -> {1}", rawFile, destinationPath);
    }

    static bool ShouldIMoveRaw(string jpgFileName, int MinimalRating = 1)
    {
        bool moveRaw = false;
        if (!File.Exists(jpgFileName))
        {
            moveRaw = true;
        }
        else
        {
            var imageFile = TagLib.File.Create(
                jpgFileName
            );
            var tag = imageFile.Tag as TagLib.Image.CombinedImageTag;
            if (tag is not null)
            {
                if (tag.Rating >= MinimalRating)
                {
                    moveRaw = true;
                }
            }
        }
        return moveRaw;
    }

    static void Main(string[] args)
    {
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        Console.WriteLine("Move not starred C# v.{0}", version);
        bool dry_run = false;
        if (args.Contains("--dry-run"))
        {
            dry_run = true;
            Console.WriteLine("Dry run mode enabled. No files will be moved.");
        }
        List<string> rawFiles = GetListOfRawFiles();
        foreach (string rawFile in rawFiles)
        {
            string rawFileName = Path.GetFileNameWithoutExtension(rawFile);
            string jpgFileName = rawFileName + ".JPG";
            if (ShouldIMoveRaw(jpgFileName))
            {
                MoveRawFile(rawFile, dry_run);
            }
            else
            {
                Console.WriteLine(rawFile);
            }
        }
    }
}