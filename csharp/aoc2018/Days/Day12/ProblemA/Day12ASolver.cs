using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day12ASolver
	{
		public const int NumGenerations = 20;

		public const char Alive = '#';
		public const char Dead = '.';

		public static char[] CurrentGeneration;
		public static char[] NextGeneration;

		public static Dictionary<string, char> Ruleset = new Dictionary<string, char>();
		
		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day12/ProblemA/input.txt");

			String[] lines = File.ReadAllLines(inputPath);

			string initialState = lines[0];
			initialState = initialState.Split(':')[1].Trim();

			// Buffer out room equal to the number of generations we want to simulate
			List<char> initialBuffer = new List<char>();
			for (int i = 0; i <= NumGenerations+1; i++)
			{
				initialBuffer.Add(Dead);	
			}
			initialBuffer.AddRange(initialState.ToCharArray());
			for (int i = 0; i <= NumGenerations+1; i++)
			{
				initialBuffer.Add(Dead);	
			}

			CurrentGeneration = initialBuffer.ToArray();
			NextGeneration = initialBuffer.ToArray();
			

			for (int i = 1; i < lines.Length; i++)
			{
				string l = lines[i].Trim();
				if (!string.IsNullOrEmpty(l))
				{
					string rule = l.Substring(0, l.IndexOf('=')).Trim();
					char outcome = l.Substring(l.IndexOf('>') + 1).Trim()[0];
					Ruleset[rule] = outcome;
				}
			}

			Log.WriteLine("Beginning simulation for initial state of size " + initialState.Length + " num generations: " + NumGenerations);
			StringBuilder sb = new StringBuilder();
			Log.WriteLine("["+0+"]" + new String(CurrentGeneration));
			for (int g = 1; g <= NumGenerations; g++)
			{
				for (int i = 0; i < CurrentGeneration.Length; i++)
				{
					if (i < 2 || i > CurrentGeneration.Length - 3)
					{
						NextGeneration[i] = CurrentGeneration[i];
						continue;
					}

					sb.Clear();
					sb.Append(CurrentGeneration[i-2]);
					sb.Append(CurrentGeneration[i-1]);
					sb.Append(CurrentGeneration[i]);
					sb.Append(CurrentGeneration[i+1]);
					sb.Append(CurrentGeneration[i+2]);
					char outcome;
					if (Ruleset.TryGetValue(sb.ToString(), out outcome))
					{
						NextGeneration[i] = outcome;
					}
					else
					{
						Log.WriteLine(i + " No rule for " + sb.ToString());
						NextGeneration[i] = Dead;
					}
					
					
				}

				NextGeneration.CopyTo(CurrentGeneration, 0);
				string turn = g.ToString();
				if (g < 10)
				{
					turn = "0" + turn;
				}
				Log.WriteLine("["+turn+"]" + new String(CurrentGeneration));
			}

			int sum = 0;
			for (int i = 0; i < CurrentGeneration.Length; i++)
			{
				if (CurrentGeneration[i] == Alive)
				{
					sum += (i - (NumGenerations+2));	
				}
			}
			Log.WriteLine("Simulation completed check sum is " + sum);
		}
		
	}
}