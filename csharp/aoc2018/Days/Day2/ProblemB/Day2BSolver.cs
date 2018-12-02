using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day2BSolver
	{
		public static void Solve()
		{
			int sum = 0;
			
			string inputPath = FileUtils.GetProjectFilePath("Days/Day2/ProblemB/input.txt");
			List<string> allStrings = new List<string>();
			using (var reader = new System.IO.StreamReader(inputPath))
			{
				string line;
				while((line = reader.ReadLine()) != null)  
				{  
					allStrings.Add(line.Trim());
				}  
			}

			bool pairFound = false;
			for (int i = 0; i < allStrings.Count; i++)
			{
				for (int j = i+1; j < allStrings.Count; j++)
				{
					if (ApproximatelyEqual(allStrings[i], allStrings[j]))
					{
						Log.WriteLine("Match found - common substring is " + GenerateCommonSubstring(allStrings[i], allStrings[j]));
						pairFound = true;
						break;
					}
				}

				if (pairFound)
				{
					break;
				}
			}

			if (!pairFound)
			{
				Log.WriteLine("No checksum found in data set");
			}
		}

		private static string GenerateCommonSubstring(string lineA, string lineB)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < lineA.Length; i++)
			{
				if (lineA[i] == lineB[i])
				{
					sb.Append(lineA[i]);
				}
			}

			return sb.ToString();
		}
		
		
		private static bool ApproximatelyEqual(string lineA, string lineB)
		{
			for (int i = 0; i < lineA.Length; i++)
			{
				bool match = true;
				for (int j = 0; j < lineA.Length; j++)
				{
					if (i == j)
					{
						continue;
					}

					if (lineA[j] != lineB[j])
					{
						match = false;
						break;
					}
				}

				if (match)
				{
					return true;
				}
			}

			return false;
		}
		
	}
}