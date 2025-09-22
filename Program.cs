using System;
using System.IO;
using System.Collections;

class Program
{
    static void Main(string[] args)
    {
        string path = Directory.GetCurrentDirectory();
        Console.WriteLine("Listing files in {0} folder", path);
        string[] fileEntries = Directory.GetFiles(".");
        foreach (string fileName in fileEntries)
        {
            Console.WriteLine(fileName);
        }
    }
}