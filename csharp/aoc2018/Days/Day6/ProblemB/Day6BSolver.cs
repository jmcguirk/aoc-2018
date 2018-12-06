using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day6BSolver
	{

		public static void Solve()
		{
			const int targetSumDistance = 10000;
			string inputPath = FileUtils.GetProjectFilePath("Days/Day6/ProblemB/input.txt");
			string[] lines = File.ReadAllLines(inputPath);
			List<Point> points = new List<Point>();
			int minX = Int32.MaxValue;
			int minY = Int32.MaxValue;
			int maxX = Int32.MinValue;
			int maxY = Int32.MinValue;
			foreach (var line in lines)
			{
				var p = new Point(line);
				if (p.X < minX)
				{
					minX = p.X;
				}
				if (p.Y < minY)
				{
					minY = p.Y;
				}
				if (p.X > maxX)
				{
					maxX = p.X;
				}
				if (p.Y > maxY)
				{
					maxY = p.Y;
				}
				points.Add(p);
			}
			Log.WriteLine("Grid is between " + minX + "," + minY + " and " + maxX + "," + maxY);
			HashSet<int> distances = new HashSet<int>();
			int region = 0;
			for (int i = minX; i <= maxX; i++)
			{
				for (int j = minY; j <= maxY; j++)
				{
					int sumDist = 0;
					foreach (var p in points)
					{
						sumDist += p.ManhattanDistance(i, j);
						
					}

					if (sumDist < targetSumDistance)
					{
						region++;
					}
				}
			}
			Log.WriteLine("Total region size is " + region);
			
			
		}
		
		public class Point
		{
			public int X;
			public int Y;
			public string Id;
			public int OwnedTerritory;

			public int ManhattanDistance(int X, int Y)
			{
				return Math.Abs(this.X - X) + Math.Abs(this.Y - Y);
			}

			public Point(string line)
			{
				string[] parts = line.Split(',');
				X = Int32.Parse(parts[0]);
				Y = Int32.Parse(parts[1]);
				if (parts.Length > 2)
				{
					Id = parts[2];
				}
			}
		}

	}
}