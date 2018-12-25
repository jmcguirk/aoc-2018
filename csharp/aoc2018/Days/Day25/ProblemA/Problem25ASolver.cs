using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Z3;
using System.Xml.Schema;

using Log=System.Console;
namespace AdventOfCode
{
	public class Day25ASolver
	{

		private static List<Point> _allPoints = new List<Point>();
		
		private static List<Constellation> _allConstellations = new List<Constellation>();
		
		private static Dictionary<Point, Constellation> _membership = new Dictionary<Point, Constellation>();
		
		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day25/ProblemA/input.txt");
			string[] lines = File.ReadAllLines(inputPath);
			int count = 0;

			for(int i = 0; i < lines.Length; i++)
			{
				string l = lines[i].Trim();
				if (!string.IsNullOrEmpty(l))
				{
					_allPoints.Add(new Point(l, count));
					count++;
				}
			}
			
			Log.WriteLine("Added " + _allPoints.Count + " points");

			const int targetDistance = 3;

			for (int i = 0; i < _allPoints.Count; i++)
			{
				for (int j = 0; j < _allPoints.Count; j++)
				{
					if (i != j)
					{
						long dist = _allPoints[i].Distance(_allPoints[j]);
						if (dist <= targetDistance)
						{
							_allPoints[i].Neighbors.Add(_allPoints[j]);
						}
					}
				}
			}

			int constellationCount = 0;
			foreach (var point in _allPoints)
			{
				Constellation existing;
				if(!_membership.TryGetValue(point, out existing))
				{
					BuildConstellation(point, constellationCount);
					constellationCount++;
				}
			}

			int nonDegenerateCount = 0;
			foreach (var c in _allConstellations)
			{
				if (c.Contents.Count > 1)
				{
					nonDegenerateCount++;
				}
			}
			
			Log.WriteLine("Found " + constellationCount + " constellations - " + nonDegenerateCount + " were non degenerate");
		}

		private static void BuildConstellation(Point point, int id)
		{
			Constellation c = new Constellation();
			c.Id = id;
			BuildConstellation(c, point);
			foreach (var p in c.Contents)
			{
				_membership[p] = c;
			}
		}
		
		private static void BuildConstellation(Constellation c, Point point)
		{
			c.Contents.Add(point);
			foreach (var p in point.Neighbors)
			{
				if (!c.Contents.Contains(p))
				{
					BuildConstellation(c, p);
				}
			}
		}

		public class Constellation
		{
			public List<Point> Contents = new List<Point>();
			public int Id;
		}

		public class Point
		{
			public long X;
			public long Y;
			public long Z;
			public long W;

			public List<Point> Neighbors = new List<Point>();

			public int Id;
			
			public Point(string line, int cnt)
			{
				Id = cnt;

				string[] parts = line.Trim().Split(',');
				X = Int64.Parse(parts[0]);
				Y = Int64.Parse(parts[1]);
				Z = Int64.Parse(parts[2]);
				W = Int64.Parse(parts[3]);
			}

			public string DebugLog()
			{
				StringBuilder sb = new StringBuilder();

				return sb.ToString();
			}
			
			public long Distance(Point b)
			{
				return Distance(this, b);
			}
			
			public static long Distance(Point a, Point b)
			{
				return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z) + Math.Abs(a.W - b.W);
			}
		}
	
	}
}