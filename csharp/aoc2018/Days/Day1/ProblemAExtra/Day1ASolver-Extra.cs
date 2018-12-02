//#define THREADED_SOLUTION
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day1AExtraSolver
	{
		
		public static void Solve()
		{
			using (var timer = new Timer("Day 1A Problem"))
			{
				int sum = 0;
				string inputPath = FileUtils.GetProjectFilePath("Days/Day1/ProblemAExtra/input.txt");
				foreach (string readLine in File.ReadLines(inputPath)) {
					int result = 0;
					if (int.TryParse(readLine, out result)) {
						sum += result;
					}    
				}
	
				Log.WriteLine("Sum is " + sum);
			}
		}
		
		#if THREADED_SOLUTION
		public static void Solve()
		{
			const int WorkSlice = 1000000;
			using (var timer = new Timer("Day 1A Problem"))
			{
				string inputPath = FileUtils.GetProjectFilePath("Days/Day1/ProblemAExtra/input.txt");
				String[] lines = File.ReadAllLines(inputPath);

				List<Task<int>> lst = new List<Task<int>>();
				int startLine = 0;
				bool foundEnd = false;
				while (true)
				{
					int endLine = startLine + WorkSlice;
					
					if (endLine >= lines.Length)
					{
						endLine = lines.Length - 1;
						foundEnd = true;
					}
					lst.Add(CreateJob(lines, startLine, endLine));
					if (foundEnd)
					{
						break;
					}

					startLine = endLine + 1;
					
				}

				Task<int>[] tasks = lst.ToArray();
				Log.WriteLine("Awaiting " + tasks.Length + " tasks");
				Task.WaitAll(tasks);
				int sum = 0;
				for (int i = 0; i < tasks.Length; i++)
				{
					sum += tasks[i].Result;
				}
				Log.WriteLine("Sum is " + sum);
			}
		}
		#endif

		public static Task<int> CreateJob(String[] lines, int startLine, int endLine)
		{
			return Task<int>.Run(() => Sum(lines, startLine, endLine));
		}

		public static int Sum(String[] lines, int startLine, int endLine)
		{
			int sum = 0;
			for(int i = startLine; i <= endLine; i++)
			{
				string trim = lines[i].Trim();
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

			return sum;
		}
	}
}