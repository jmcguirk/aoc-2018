using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day3BSolver
	{
		private static int TileMapSize = 1000;
		private static int[] TileMap = new int[TileMapSize * TileMapSize];
		private static HashSet<int> Overlaps = new HashSet<int>();
		
		public static void Solve()
		{
			int sum = 0;
			
			string inputPath = FileUtils.GetProjectFilePath("Days/Day3/ProblemB/input.txt");

			using (var reader = new System.IO.StreamReader(inputPath))
			{
				string line;
				List<Rect> allRects = new List<Rect>();
				while((line = reader.ReadLine()) != null)
				{
					string input = line.Trim();
					if (!String.IsNullOrEmpty(input))
					{
						var rect = new Rect(input);
						MarkUsage(rect);
						allRects.Add(rect);
					}
				}
				foreach (var rect in allRects)
				{
					if (IsIsolated(rect))
					{
						Console.WriteLine(rect.Id + " had no overlaps");
					}
				}
			}
		}

		private static bool IsIsolated(Rect rect)
		{
			
			for (int i = rect.MinX; i < (rect.MinX + rect.Width); i++)
			{
				for (int j = rect.MinY; j < (rect.MinY + rect.Height); j++)
				{
					int index = TileToIndex(i, j);
					if (TileMap[index] > 1)
					{
						return false;
					}
				}
			}

			return true;
		}

		private static int TileToIndex(int x, int y)
		{
			return (x * TileMapSize) + y;
		}

		private static void MarkUsage(Rect rect)
		{
			
			for (int i = rect.MinX; i < (rect.MinX + rect.Width); i++)
			{
				for (int j = rect.MinY; j < (rect.MinY + rect.Height); j++)
				{
					MarkTileUsed(rect, i, j);
				}
			}
		}

		private static void MarkTileUsed(Rect rect, int x, int y)
		{
			int index = TileToIndex(x, y);
			if (TileMap[index] == 0)
			{
				TileMap[index] = 1;
			}
			else
			{
				TileMap[index]++;
				if (!Overlaps.Contains(index))
				{
					Overlaps.Add(index);
				}
			}
		}

		private class Rect
		{
			public int MinX;
			public int MinY;
			public int Width;
			public int Height;
			public string Id;

			public Rect(string rawInput)
			{
				string[] idParts = rawInput.Split('@');
				Id = idParts[0].Trim();
				string[] coordParts = idParts[1].Trim().Split(':');
				string[] minparts = coordParts[0].Trim().Split(',');
				string[] extentParts = coordParts[01].Trim().Split('x');
				MinX = Int32.Parse(minparts[0]);
				MinY = TileMapSize - Int32.Parse(minparts[1]); // Flip y
				Width = Int32.Parse(extentParts[0]);
				Height = Int32.Parse(extentParts[1]);
				MinY -= Height; // Since our coords are given as top of rect
			}
			
			
		}
		
	}
}