using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Xmp;

class move_not_starred
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
            Console.WriteLine("True 46");
            return true;
        }
        else
        {

            try
            {
                IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(jpgFileName);
                var ifd0Directory = directories.OfType<MetadataExtractor.Formats.Exif.ExifIfd0Directory>().FirstOrDefault();
                var ifd0Rating = ifd0Directory.TryGetInt32(ExifIfd0Directory.TagRating, out var iRating) ? iRating : 0;
                var exifDirectory = directories.OfType<MetadataExtractor.Formats.Exif.ExifDirectoryBase>().FirstOrDefault();
                var exifRating = exifDirectory.TryGetInt32(ExifDirectoryBase.TagRating, out var eRating) ? eRating : 0;
                var xmpDirectory = directories.OfType<MetadataExtractor.Formats.Xmp.XmpDirectory>().FirstOrDefault();
                var xmpRating = xmpDirectory.TryGetInt32(XmpDirectory.TagXmpValueCount, out var xRating) ? xRating : 0;
                
                var taglibRating = 0;
                try
                {
                    var tfile = TagLib.File.Create(jpgFileName);
                    var image = tfile as TagLib.Image.File;
                    taglibRating = (int)image.ImageTag.Rating;
                }
                catch (NotImplementedException)
                {

                }
                
                Console.WriteLine("{0} -> {1}, {2}, {3}, {4}", jpgFileName, exifRating, xmpRating, ifd0Rating, taglibRating);
                // Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
                // foreach (var directory in directories)
                //     foreach (var tag in directory.Tags)
                //         Console.WriteLine($"     {directory.Name} - {tag.Name} = {tag.Description};");
                if (exifRating < minimalRating)
                {
                    return true;
                }
            }
            catch (NullReferenceException) {
                // move raw files, for jpegs that cannot be processed
                return true;
            }

        }
        return false;
    }

    static void Main(string[] args)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".0.0", "");
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
            else
            {
                Console.WriteLine(rawFile);
            }
        }
        Console.WriteLine("{0} files moved out of {1}", filesMoved, rawFiles.Count);
    }
}