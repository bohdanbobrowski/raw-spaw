using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

internal class raw_spaw
{
    private static List<string> GetListOfRawFiles()
    {
        var rawFiles = new List<string>();
        var path = Directory.GetCurrentDirectory();
        Console.WriteLine("Listing files in {0} folder", path);
        var fileEntries = Directory.GetFiles(".");
        foreach (var fileName in fileEntries)
            if (fileName.EndsWith(".DNG"))
                rawFiles.Add(fileName);

        return rawFiles;
    }

    private static void MoveRawFile(string rawFile, bool dryRun = false)
    {
        var rawFileName = Path.GetFileNameWithoutExtension(rawFile);
        var dirName = new DirectoryInfo(".").Name;
        var destinationPath = Path.Combine(dirName, rawFileName + ".DNG");
        if (!Directory.Exists(dirName) & !dryRun)
        {
            Console.WriteLine("Create directory: {0}", dirName);
            Directory.CreateDirectory(dirName);
        }

        if (!dryRun)
            File.Move(rawFile, destinationPath);
        Console.WriteLine("{0} -> {1}", rawFile, destinationPath);
    }

    private static bool ShouldIMoveRaw(string jpgFileName, int minimalRating = 1)
    {
        if (!File.Exists(jpgFileName)) return true;

        var exifRating = 0;
        try
        {
            var tfile = TagLib.File.Create(jpgFileName);
            var image = tfile as TagLib.Image.File;
            exifRating = (int)image.ImageTag.Rating;
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine(jpgFileName + " is not ranked");
            return true;
        }
        catch (NotImplementedException)
        {
            Console.WriteLine(jpgFileName + " is not ranked");
            return true;
        }

        Console.WriteLine("{0} {1}", jpgFileName, GetStars(exifRating));
        if (exifRating < minimalRating) return true;

        return false;
    }

    private static string GetStars(int count, int range = 5)
    {
        var starsString = "";
        for (var index = 1; index <= count; index++) starsString += " ☆";

        return starsString;
    }

    private static string Version()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        while (version.EndsWith(".0"))
        {
            version = version.Substring(0, version.Length - 2);
        }

        return version;
    }

    private static void PrintInformation()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Move not starred C# v.{0}", Version());
    }

    private static void PrintHelp()
    {
        Console.WriteLine(@"usage: raw_spaw [-h] [-d] [-p PICTURE_EXTENSION] [-r RAW_EXTENSION] [-t TARGET]
Move raw files for jpegs that are not starred to subfolder. Requires exiftool.
    options:
-h, --help            show this help message and exit
-d, --dry-run         Dry run.
-p PICTURE_EXTENSION, --picture_extension PICTURE_EXTENSION
    Picture file extension (by default JPG).
-r RAW_EXTENSION, --raw_extension RAW_EXTENSION
    RAW file extension (by default DNG).
-t TARGET, --target TARGET
    Target path (by default creates sub folder with the same name as current one).
");
    }

    private static void Main(string[] args)
    {
        PrintInformation();
        var dryRun = false;
        if (args.Contains("-h") || args.Contains("--help"))
        {
            PrintHelp();
            return;
        }
        if (args.Contains("-d") || args.Contains("--dry-run"))
        {
            dryRun = true;
            Console.WriteLine("Dry run mode enabled. No files will be moved.");
        }

        var filesMoved = 0;
        var rawFiles = GetListOfRawFiles();
        foreach (var rawFile in rawFiles)
        {
            var rawFileName = Path.GetFileNameWithoutExtension(rawFile);
            var jpgFileName = rawFileName + ".JPG";
            if (ShouldIMoveRaw(jpgFileName))
            {
                MoveRawFile(rawFile, dryRun);
                filesMoved += 1;
            }
        }

        Console.WriteLine("{0} files moved out of {1}", filesMoved, rawFiles.Count);
    }
}