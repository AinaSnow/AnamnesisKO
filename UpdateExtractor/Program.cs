﻿// Concept Matrix 3.
// Licensed under the MIT license.

namespace UpdateExtractor
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Threading;

	public class Program
	{
		public static void Main(string[] args)
		{
			string targetPath = args[0];
			string processName = args[1];
			Console.WriteLine($"{processName} Updater");

			try
			{
				Console.Write($"Waiting for {processName} to terminate");
				while (true)
				{
					Process[] procs = Process.GetProcessesByName(processName);
					if (procs.Length <= 0)
					{
						break;
					}
					else
					{
						Thread.Sleep(100);
						Console.Write(".");
					}
				}

				Console.WriteLine(" done.");

				string? destDir = Path.GetDirectoryName(targetPath) + "/";
				string? currentExePath = System.AppContext.BaseDirectory;
				string? sourceDir = Path.GetDirectoryName(currentExePath) + "/../";

				if (string.IsNullOrEmpty(currentExePath))
					throw new Exception("Unable to determine current process path");

				if (string.IsNullOrEmpty(sourceDir))
					throw new Exception("Unable to determine source directory");

				if (string.IsNullOrEmpty(destDir))
					throw new Exception("Unable to determine destination directory");

				if (!File.Exists(destDir + "Anamnesis.exe"))
					throw new Exception($"No Anamnesis executable found in destination directory: {destDir}");

				Console.WriteLine("Cleaning old version");
				DeleteFileIfExists(destDir + "Anamnesis.exe");
				DeleteFileIfExists(destDir + "Anamnesis.pdb");
				DeleteFileIfExists(destDir + "Anamnesis.xml");
				DeleteFileIfExists(destDir + "Version.txt");

				DeleteDirectoryIfExists(destDir + "Data");
				DeleteDirectoryIfExists(destDir + "Languages");
				DeleteDirectoryIfExists(destDir + "Updater");

				Console.WriteLine("Copying Update Files");

				string[] files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
				foreach (string sourceFile in files)
				{
					string destFile = sourceFile.Replace(sourceDir, destDir);
					Console.WriteLine("    > " + destFile);

					string? directory = Path.GetDirectoryName(destFile);
					if (directory != null && !Directory.Exists(directory))
						Directory.CreateDirectory(directory);

					File.Copy(sourceFile, destFile, true);
				}

				Console.WriteLine("Restarting application");
				Console.WriteLine("    > " + targetPath);
				ProcessStartInfo start = new ProcessStartInfo(targetPath);
				Process.Start(start);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Failed to update {processName}");
				Console.WriteLine(ex.Message);
				Console.WriteLine();
				Console.WriteLine("Please try again or download the update manually.");
				Console.WriteLine();
				Console.WriteLine("Press any key to close this window.");
				Console.ReadKey();
			}
		}

		private static void DeleteFileIfExists(string path)
		{
			if (!File.Exists(path))
				return;

			File.Delete(path);
		}

		private static void DeleteDirectoryIfExists(string path)
		{
			if (!Directory.Exists(path))
				return;

			Directory.Delete(path, true);
		}
	}
}
