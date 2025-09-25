using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Xmp;

class raw_spaw
{
    static List<string> GetListOfRawFiles()
    {
        List<string> rawFiles = new List<string>();
        string path = System.IO.Directory.GetCurrentDirectory();
        Console.WriteLine("Listing files in {0} folder", path);
        string[] fileEntries = System.IO.Directory.GetFiles(".");
        foreach (string fileName in fileEntries)
        {
            if (fileName.EndsWith(".DNG"))
                rawFiles.Add(fileName);
        }

        return rawFiles;
    }

    static void MoveRawFile(string rawFile, bool dryRun = false)
    {
        var rawFileName = Path.GetFileNameWithoutExtension(rawFile);
        var dirName = new DirectoryInfo(".").Name;
        var destinationPath = Path.Combine(dirName, rawFileName + ".DNG");
        if (!System.IO.Directory.Exists(dirName) & !dryRun)
        {
            Console.WriteLine("Create directory: {0}", dirName);
            System.IO.Directory.CreateDirectory(dirName);
        }

        if (!dryRun)
            File.Move(rawFile, destinationPath);
        Console.WriteLine("{0} -> {1}", rawFile, destinationPath);
    }

    static bool ShouldIMoveRaw(string jpgFileName, int minimalRating = 1)
    {
        if (!File.Exists(jpgFileName))
        {
            return true;
        }

        var exifRating = 0;
        try
        {
            var tfile = TagLib.File.Create(jpgFileName);
            var image = tfile as TagLib.Image.File;
            exifRating = (int)image.ImageTag.Rating;
        }
        catch (NotImplementedException)
        {
            Console.WriteLine(jpgFileName + " is not ranked");
            return true;
        }

        Console.WriteLine("{0} {1}", jpgFileName, GetStars(exifRating))  ;
        if (exifRating < minimalRating)
        {

            return true;
        }

        return false;
    }

    static string GetStars(int count, int range = 5)
    {
        var starsString = "";
        for (int index=1; index<=count; index++)
        {
            starsString += " ☆";
        }
        return starsString; 
    }

    static void Main(string[] args)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".0.0", "");
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Move not starred C# v.{0}", version);
        var dryRun = false;
        if (args.Contains("--dry-run"))
        {
            dryRun = true;
            Console.WriteLine("Dry run mode enabled. No files will be moved.");
        }

        var filesMoved = 0;
        List<string> rawFiles = GetListOfRawFiles();
        foreach (string rawFile in rawFiles)
        {
            string rawFileName = Path.GetFileNameWithoutExtension(rawFile);
            string jpgFileName = rawFileName + ".JPG";
            if (ShouldIMoveRaw(jpgFileName))
            {
                MoveRawFile(rawFile, dryRun);
                filesMoved += 1;
            }
        }
        Console.WriteLine("{0} files moved out of {1}", filesMoved, rawFiles.Count);
    }
}