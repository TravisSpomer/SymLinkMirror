using System;
using System.IO;
using SymLinkMirror;

Console.WriteLine("SymLinkMirror");
Console.WriteLine("(c) 2011-2021 Travis Spomer");
Console.WriteLine();

// First, check the arguments.
if (args.Length != 2)
{
	if (args.Length != 0)
	{
		Console.WriteLine("Error: You must supply exactly two arguments.");
		Console.WriteLine();
	}
	PrintUsage();
	return (int)ReturnCode.InvalidArguments;
}

string sourcePath = args[0];
DirectoryInfo source;
try
{
	source = new DirectoryInfo(sourcePath);
}
catch (DirectoryNotFoundException)
{
	Console.WriteLine("Error: The source folder wasn't found.");
	Console.WriteLine();
	return (int)ReturnCode.NotFound;
}

string destPath = args[1];

// Okay, perform the mirror operation now.
Console.WriteLine("Mirroring:");
Console.Write("    ");
Console.WriteLine(sourcePath);
Console.WriteLine("to:");
Console.Write("    ");
Console.WriteLine(destPath);
Console.WriteLine();
source.MirrorUsingSymLinks(destPath);

// Success.  Yay for us!
Console.WriteLine("Done.");
Console.WriteLine();
return (int)ReturnCode.Success;


static void PrintUsage()
{
	Console.WriteLine("Mirrors a folder structure from one location to another using symbolic");
	Console.WriteLine("links instead of copying files.");
	Console.WriteLine();
	Console.WriteLine("Usage:");
	Console.WriteLine(@"    SymLinkMirror ""Source path"" ""Destination path""");
	Console.WriteLine();
	Console.WriteLine("Symbolic links must be created from an administrator command prompt.");
	Console.WriteLine();
}

namespace SymLinkMirror
{
	public enum ReturnCode : int
	{
		Success = 0,
		InvalidArguments = 1,
		NotFound = 2,
	}
}
