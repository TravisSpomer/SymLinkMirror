using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SymLinkMirror
{
	public static class SymLinkUtilities
	{
		// ------------------------------------------------------------
		// API declarations
		// ------------------------------------------------------------

		private enum SymbolicLinkFlag : uint
		{
			File = 0,
			Directory = 1,
		}

		[DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLinkFlag dwFlags);

		// ------------------------------------------------------------
		// Custom exceptions
		// ------------------------------------------------------------

		/// <summary>
		/// The exception that is thrown when an error occurs while creating a symbolic link.
		/// </summary>
		public sealed class SymLinkException : IOException
		{
			public int Win32ErrorCode
			{
				get
				{
					return _Win32ErrorCode;
				}
			}
			private readonly int _Win32ErrorCode = 0;

			public SymLinkException(int win32ErrorCode) : base("Creation of the symbolic link failed. You may not have permission to create a symbolic link at the destination. Creation of symbolic links requires administrator privileges and elevation.") { _Win32ErrorCode = win32ErrorCode; }

			public override string Message
			{
				get
				{
					return base.Message + "  (" + _Win32ErrorCode.ToString() + ")";
				}
			}
		}

		// ------------------------------------------------------------
		// Extension methods
		// ------------------------------------------------------------

		/// <summary>
		/// Recursively mirrors a directory structure to another location by creating symbolic links instead of copying files.
		/// </summary>
		/// <param name="source">The source directory to mirror.</param>
		/// <param name="destination">The destination where symbolic links will be created.</param>
		public static void MirrorUsingSymLinks(this DirectoryInfo source, string destination)
		{
			// First, make sure that the destination folder exists.
			if (!Directory.Exists(destination))
				Directory.CreateDirectory(destination);

			// First, mirror all files in this folder.
			foreach (FileInfo sourceFile in source.EnumerateFiles())
				sourceFile.SymLinkTo(Path.Combine(destination, sourceFile.Name));

			// Then, recursively mirror all of the subfolders of this folder.
			foreach (DirectoryInfo sourceFolder in source.EnumerateDirectories())
				sourceFolder.MirrorUsingSymLinks(Path.Combine(destination, sourceFolder.Name));
		}

		/// <summary>
		/// Creates a file symbolic link.
		/// </summary>
		/// <param name="source">The existing file to link to.</param>
		/// <param name="destination">The path to the symbolic link to be created.</param>
		/// <remarks>
		/// If the destination file already exists, it will be deleted before the symbolic link is created.
		/// File attributes and access times will also be mirrored, but ACLs will not be.
		/// </remarks>
		public static void SymLinkTo(this FileInfo source, string destinationPath)
		{
			if (File.Exists(destinationPath))
				File.Delete(destinationPath);
			if (!CreateSymbolicLink(destinationPath, source.FullName, SymbolicLinkFlag.File))
				throw new SymLinkException(Marshal.GetLastWin32Error());
			//source.CopyAttributesAndTimestampsTo(new FileInfo(destinationPath));
		}

		/// <summary>
		/// Creates a directory symbolic link.
		/// </summary>
		/// <param name="source">The existing directory to link to.</param>
		/// <param name="destination">The path to the symbolic link to be created.</param>
		/// <remarks>If the destination directory already exists, this method will fail.</remarks>
		public static void SymLinkTo(this DirectoryInfo source, string destinationPath)
		{
			if (!CreateSymbolicLink(destinationPath, source.FullName, SymbolicLinkFlag.Directory))
				throw new SymLinkException(Marshal.GetLastWin32Error());
			//source.CopyAttributesAndTimestampsTo(new DirectoryInfo(destinationPath));
		}

		/// <summary>
		/// Copies the file system attributes, creation time, last write time, and last access time from
		/// one FileSystemInfo object to another of the same type.
		/// </summary>
		/// <param name="source">The source of the file system attributes to copy.</param>
		/// <param name="destination">The destination object whose attributes are to be changed.</param>
		public static void CopyAttributesAndTimestampsTo(this FileSystemInfo source, FileSystemInfo destination)
		{
			if (destination == null)
				throw new ArgumentNullException(nameof(destination));
			bool sourceIsFile = source is FileInfo;
			bool destIsFile = destination is FileInfo;
			if ((sourceIsFile && !destIsFile) || (!sourceIsFile && destIsFile))
				throw new ArgumentOutOfRangeException(nameof(destination), "The source and destination FileSystemInfo objects must be of the same derived type.");

			destination.Attributes = source.Attributes;
			destination.CreationTimeUtc = source.CreationTimeUtc;
			destination.LastWriteTimeUtc = source.LastWriteTimeUtc;
			destination.LastAccessTimeUtc = source.LastAccessTimeUtc;
		}

	}

}