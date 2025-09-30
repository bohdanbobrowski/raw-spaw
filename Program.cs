using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Directory = System.IO.Directory;
using CommandLine;


public class RawSpawOptions
{
    [Option('d', "dry-run", Required = false, Default = false, HelpText = "Dry run."),]
    public bool DryRun { get; set; }

    [Option('p', "picture-extension", Required = false, Default = "JPG", HelpText = "Picture file extension."),]
    public string PictureExtension { get; set; }

    [Option('r', "raw-extension", Required = false, Default = "DNG", HelpText = "RAW file extension."),]
    public string RawExtension { get; set; }

    [Option('t', "target", Required = false, Default = ".", HelpText = "Target path."),]
    public string Target { get; set; }
    
    [Option('i', "interactive", Required = false, Default = false, HelpText = "Interactive mode."),]
    public bool Interactive { get; set; }
}

internal class RawSpaw
{
    private static List<string> GetListOfRawFiles(string rawExtension = "DNG")
    {
        var path = Directory.GetCurrentDirectory();
        Console.WriteLine("Listing files in {0} folder", path);
        var fileEntries = Directory.GetFiles(".");
        
        return fileEntries.Where(fileName => fileName.ToUpper().EndsWith("." + rawExtension.ToUpper())).ToList();
    }

    private static void MoveRawFile(string rawFile, bool dryRun = false, string target = ".")
    {
        var dirName = new DirectoryInfo(".").Name;
        string[] paths = [target, dirName];
        var targetDir = Path.Combine(paths);
        var destinationPath = Path.Combine(targetDir, rawFile);
        if (!Directory.Exists(targetDir) & !dryRun)
        {
            Console.WriteLine("Create directory: {0}", targetDir);
            Directory.CreateDirectory(targetDir);
        }

        if (!dryRun)
            File.Move(rawFile, destinationPath);
        Console.Write("{0} -> {1}\n", rawFile, destinationPath);
    }

    private static int GetRatingWithMetadataExtractor(string jpgFileName)
    {
        IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(jpgFileName);
        var exifDirectory = directories.OfType<MetadataExtractor.Formats.Exif.ExifDirectoryBase>().FirstOrDefault();
        var exifRating = exifDirectory.TryGetInt32(ExifDirectoryBase.TagRating, out var eRating) ? eRating : 0;
        return exifRating == null ? 0 : exifRating;
    }

    private static int GetRatingWithTagLib(string jpgFileName)
    {
        // In some cases (needs deeper debugging) MetadataExtractor fails to get picture rating, so I'm helping myself
        // with TagLib.
        var tfile = TagLib.File.Create(jpgFileName);
        var image = tfile as TagLib.Image.File;
        return (int)image.ImageTag.Rating;
    }

    private static bool ShouldIMoveRaw(string jpgFileName, int minimalRating = 1)
    {
        if (!File.Exists(jpgFileName)) return true;
        var exifRating = 0;
        try
        {
            exifRating = GetRatingWithMetadataExtractor(jpgFileName);
        }
        catch (InvalidOperationException)
        {
            try
            {
                exifRating = GetRatingWithTagLib(jpgFileName);
            }
            catch (NotImplementedException)
            {
                Console.WriteLine(jpgFileName + " is not ranked");
                return true;
            }
        }

        Console.Write("{0}\t{1}", jpgFileName, GetStars(exifRating));
        if (exifRating < minimalRating)
        {
            return true;
        }

        Console.Write("\n");
        return false;
    }

    private static string GetStars(int count)
    {
        var starsString = "";
        for (var index = 1; index <= count; index++) starsString += " ☆";

        return starsString;
    }

    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Parser.Default.ParseArguments<RawSpawOptions>(args)
            .WithParsed<RawSpawOptions>(o =>
            {
                if (o.DryRun)
                {
                    Console.WriteLine("Dry run mode enabled. No files will be moved.");
                }

                var filesMoved = 0;
                var rawFiles = GetListOfRawFiles(o.RawExtension);
                foreach (var rawFile in rawFiles)
                {
                    var rawFileName = Path.GetFileNameWithoutExtension(rawFile);
                    var jpgFileName = rawFileName + "." + o.PictureExtension;
                    if (!ShouldIMoveRaw(jpgFileName)) continue;
                    MoveRawFile(rawFile, o.DryRun, o.Target);
                    filesMoved += 1;
                }

                Console.WriteLine("{0} files moved out of {1}", filesMoved, rawFiles.Count);

                if (o.Interactive)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(); 
                }
                
            });
    }
}