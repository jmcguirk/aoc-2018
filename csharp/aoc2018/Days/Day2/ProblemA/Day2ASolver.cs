using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day2ASolver
	{
		public static void Solve()
		{
			int sum = 0;
			
			string inputPath = FileUtils.GetProjectFilePath("Days/Day2/ProblemA/input.txt");
			int[] histogramBuffer = new int[26]; // Simplifying assumption, our input is all lower case and only alphabetical
			int twoCountSum = 0;
			int threeCountSum = 0;

			bool foundTwoCount;
			bool foundThreeCount;
			using (var reader = new System.IO.StreamReader(inputPath))
			{
				string line;
				while((line = reader.ReadLine()) != null)  
				{  
					GenerateChecksumOutputs(line.Trim(), histogramBuffer, out foundTwoCount, out foundThreeCount);
					if (foundTwoCount)
					{
						twoCountSum++;
					}

					if (foundThreeCount)
					{
						threeCountSum++;
					}
				}  
			}
			Log.WriteLine("Checksum is " + twoCountSum * threeCountSum);
		}
		
		private static void GenerateChecksumOutputs(string line, int[] histogramBuffer, out bool isTwoCount, out bool isThreeCount)
		{
			const int CharIntegerOffset = 97; // Normalize for lower case characters 
			for (int i = 0; i < line.Length; i++)
			{
				histogramBuffer[line[i] - CharIntegerOffset]++;
			}
			
			
			isTwoCount = false;
			isThreeCount = false;
			
			// Reset histogram and figure out checksum outputs
			for (int i = 0; i < histogramBuffer.Length; i++)
			{
				if (histogramBuffer[i] == 2)
				{
					isTwoCount = true;
				} else if (histogramBuffer[i] == 3)
				{
					isThreeCount = true;
				}
				histogramBuffer[i] = 0;
			}

		}
		
	}
}