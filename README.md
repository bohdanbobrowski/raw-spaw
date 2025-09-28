<img src="Assets/RawSpaw_logo_web.svg" width="300" />

# Raw-Spaw!

C sharp implementation of this [python script](https://github.com/bohdanbobrowski/python_toolbox?tab=readme-ov-file#move_not_starred).

It helps to minimize size of images folder, by moving RAW's for not ranked and ranked less than 1 star JPEG's to subfolder. 

## Usage
```bash
rawspaw [-h] [-d] [-p PICTURE_EXTENSION] [-r RAW_EXTENSION] [-t TARGET]
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
```

## Libraries used

1. [MetadataExtractor](https://github.com/drewnoakes/metadata-extractor-dotnet)
2. [TagLib](https://github.com/mono/taglib-sharp) - in cases where MetadataExtractor fails (more testing needed)

## Roadmap
- [X] Listing RAW files in folder
- [X] Creating subfolder if not exists
- [X] Reading EXIF ratings
- [X] Filtering not starred files
- [X] Moving RAW files to subfolder
- [X] Dry run option
- [X] Release as one exe file
- [X] Set release version number and print it in console
- [ ] Add commandline parser ([CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils), [commandline](https://github.com/commandlineparser/commandline)...?) 
- [ ] Customizable picture and raw extensions
- [ ] Customizable target path
- [ ] Automatic tests?
- [ ] Windows installer?
- [ ] Linux support and package
- [ ] MacOS support?

## License
MIT License
