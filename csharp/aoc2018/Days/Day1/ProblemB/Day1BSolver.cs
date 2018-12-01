using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day1BSolver
	{
		private static SortedSet<int> _seen = new SortedSet<int>();
		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day1/ProblemB/input.txt");
			String[] lines = File.ReadAllLines(inputPath);
			int sum = 0;
			bool found = false;
			int iter = 0;
			while (!found)
			{
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

							if (_seen.Contains(sum))
							{
								Log.WriteLine("Found a duplicate at " + sum);
								found = true;
								break;
							}
							else
							{
								_seen.Add(sum);
							}
						
						}
					}
				}
				iter++;
				if (found)
				{
					break;
				}
				else
				{
					Log.WriteLine("No duplicates found after " + iter + " iterations");
				}
				
			}			
		}
		
		
	}
}