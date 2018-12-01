using System;
using System.IO;
using System.Reflection;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day1ASolver
	{
		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day1/ProblemA/input.txt");
			String[] lines = File.ReadAllLines(inputPath);
			int sum = 0;
			foreach (var line in lines)
			{
				string trim = line.Trim();
				if (!String.IsNullOrEmpty(trim))
				{
					bool isPos = trim[0] == '+';
					int parsed;
					if (Int32.TryParse(trim.Substring(1), out parsed))
					{
						if (isPos)
						{
							sum += parsed;
						}
						else
						{
							sum -= parsed;
						}
						
					}
				}
			}
			Log.WriteLine("Sum is " + sum);
		}
		
		
	}
}