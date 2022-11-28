# SymLinkMirror

Mirrors a folder from one location to another using symbolic links instead of copying files.

## Usage

* Only Windows is supported.
* Install [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0) if not already installed.
* Open an **elevated** administrator command prompt.
* `SymLinkMirror "C:\Folder1" "C:\Folder2"`

## Building

If you're not using Visual Studio, you can build a release EXE with the following ([.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) is required):

```
dotnet build --configuration Release
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true --self-contained false
```

---
© 2011-2022 Travis Spomer. [MIT license](License.txt).
