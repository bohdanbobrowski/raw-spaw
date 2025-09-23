using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Xmp;

class Program
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
        string rawFileName = Path.GetFileNameWithoutExtension(rawFile);
        var dirName = new DirectoryInfo(".").Name;
        string destinationPath = Path.Combine(dirName, rawFileName + ".DNG");
        if (!System.IO.Directory.Exists(dirName))
        {
            if (!dryRun)
                System.IO.Directory.CreateDirectory(dirName);
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

            try
            {
                IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(jpgFileName);
                var ifd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                var xmpDirectory = directories.OfType<MetadataExtractor.Formats.Xmp.XmpDirectory>().FirstOrDefault();
                int xmpRating = xmpDirectory.TryGetInt32(XmpDirectory.TagXmpValueCount, out int rating) ? rating : 0;
                if (xmpRating < MinimalRating)
                {
                    moveRaw = true;
                }
            }
            catch (NullReferenceException) {
                // move raw files, for jpegs that cannot be processed
                moveRaw = true;
            }

        }
        return moveRaw;
    }

    static void Main(string[] args)
    {
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        version = version.Replace(".0.0", "");
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