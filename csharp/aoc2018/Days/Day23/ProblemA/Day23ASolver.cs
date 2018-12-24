using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day23ASolver
	{

		public static List<Nanobot> AllBots = new List<Nanobot>();
		
		public static void Solve()
		{	
			string inputPath = FileUtils.GetProjectFilePath("Days/Day23/ProblemA/input.txt");
			string[] lines = File.ReadAllLines(inputPath);
			int count = 0;
			foreach (var line in lines)
			{
				string l = line.Trim();
				if (!string.IsNullOrEmpty(l))
				{
					AllBots.Add(new Nanobot(l, count));
					count++;
				}
			}
			
			Log.WriteLine("Loaded "+ AllBots.Count + " bots");

			foreach (var bot in AllBots)
			{
				foreach (var candidate in AllBots)
				{
					bot.AddNeighborIfInRange(candidate);
				}
			}
			
			AllBots.Sort(CompareByRange);
			
			Log.WriteLine("Strongest bot is "+ AllBots[0].Debug());
		}

		private static int CompareByNeighborCount(Nanobot x, Nanobot y)
		{
			return x.NeighborCount.CompareTo(y.NeighborCount);
		}
		
		
		private static int CompareByRange(Nanobot x, Nanobot y)
		{
			return y.Range.CompareTo(x.Range);
		}


		public class Nanobot
		{
			public long X;
			public long Y;
			public long Z;
			public long Range;

			public int Id;

			public List<Nanobot> Neighbors = new List<Nanobot>();

			public Nanobot(string line, int id)
			{
				Id = id;

				string[] parts = line.Split('>');

				string[] posParts = parts[0].Split('<')[1].Split(',');
				X = Int64.Parse(posParts[0]);
				Y = Int64.Parse(posParts[1]);
				Z = Int64.Parse(posParts[2]);

				Range = Int64.Parse(parts[1].Split('=')[1]);
			}

			public void AddNeighborIfInRange(Nanobot b)
			{
				if (Distance(this, b) <= Range)
				{
					Neighbors.Add(b);
				}
			}
			
			public string Debug()
			{
				return Id + ": pos=<" + X + "," + Y + "," + Z + ">, r=" + Range + " " + NeighborCount;
			}
			
			public int NeighborCount
			{
				get { return Neighbors.Count; }
			}

			public static long Distance(Nanobot a, Nanobot b)
			{
				return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
			}
		}
	}
}