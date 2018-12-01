using System;
using System.IO;
using System.Reflection;

namespace AdventOfCode
{
	public class FileUtils
	{

		public static string GetProjectFilePath(string localFilePath)
		{
			return Path.Combine(AssemblyDirectory, localFilePath);
		}
		
		public static string AssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}
	}
}