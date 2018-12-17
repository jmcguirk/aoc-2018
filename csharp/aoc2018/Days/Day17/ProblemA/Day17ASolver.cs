using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day17ASolver
	{
		private static Tile[] _tiles;
		private static int _maxX;
		private static int _maxY;
		
		public static void Solve()
		{


			_maxX = 1000;
			
			
			string inputPath = FileUtils.GetProjectFilePath("Days/Day17/ProblemA/input.txt");
			String[] lines = File.ReadAllLines(inputPath);

			List<Vein> veins = new List<Vein>();
			Int32 deepestY = Int32.MinValue;
			foreach (var line in lines)
			{
				var l = line.Trim();
				if (!string.IsNullOrEmpty(l))
				{
					Vein v = new Vein(l);
					v.DebugLog();
					if (v.MaxY > deepestY)
					{
						deepestY = v.MaxY;
					}

					veins.Add(v);
				}
			}
			
			Log.WriteLine("Parsed " + veins.Count + " lowest vein was " + deepestY);
			_maxY = deepestY + 1;

			_tiles = new Tile[_maxX * _maxY];

			for (int i = 0; i < _maxX; i++)
			{
				for (int j = 0; j < _maxY; j++)
				{
					Tile t = new Tile();
					t.TileIndex = GetTileIndex(i, j);
					t.X = i;
					t.Y = j;
					t.TileType = TileType.Sand;
					_tiles[t.TileIndex] = t;
				}
			}

			for (int i = 0; i < _tiles.Length; i++)
			{
				var t = _tiles[i];
				t.Left = GetTile(t.X - 1, t.Y);
				t.Bottom = GetTile(t.X, t.Y + 1);
				t.Right = GetTile(t.X + 1, t.Y);
			}


			foreach (var vein in veins)
			{
				for (int i = vein.MinX; i <= vein.MaxX; i++)
				{
					for (int j = vein.MinY; j <= vein.MaxY; j++)
					{
						GetTile(i, j).TileType = TileType.Clay;
					}	
				}
			}

			
			var fountain = GetTile(500, 0);
			fountain.TileType = TileType.Fountain;

			Flood(fountain, null);
			RenderGameBoard();
			
			Log.WriteLine("Total tiles flooded: " + FloodCount);
		}

		private static int FloodCount = 0;

		private static bool BottomDiscovered = false;

		private static bool Flood(Tile root, Tile direction)
		{
			if (root.IsFlooded)
			{
				return false;
			}

			
			
			if (root.TileType == TileType.Sand && !root.IsFlooded)
			{
				root.IsFlooded = true;
				root.FloodedFrom = direction;
				root.FloodedOnFrame = FloodCount;
				FloodCount++;
			}
			


			if (root.Y == _maxY - 1)
			{
				return true;
			}


            bool bottomDiscovered = false;
			if (root.Bottom != null && !root.Bottom.IsFlooded && root.Bottom.TileType == TileType.Sand)
			{
				bottomDiscovered = Flood(root.Bottom, root);
			}

			bool didExploreLeft = false;
			if (!bottomDiscovered && root.Left != null && !root.Left.IsFlooded && root.Left.TileType == TileType.Sand)
			{
				didExploreLeft = true;
				bottomDiscovered = Flood(root.Left, root);
			}
			
			if ((root.Right != null && !root.Right.IsFlooded && root.Right.TileType == TileType.Sand) && (!bottomDiscovered || didExploreLeft))
			{
				bottomDiscovered = Flood(root.Right, root);
			}

			return bottomDiscovered;
		}

		public static void RenderGameBoard()
		{
			StringBuilder sb = new StringBuilder();
			for (int y = 0; y < _maxY; y++)
			{
				for (int x = 494; x <= 507; x++)
				{
					var tile = GetTile(x, y);
					sb.Append(tile.Render());
				}

				sb.Append(Environment.NewLine);
			}

			Log.WriteLine(sb.ToString());
		}
		
		public static int GetTileIndex(int x, int y)
		{
			return (_maxX * y) + x;
		}


		public static Tile GetTile(int x, int y)
		{
			if (x < 0 || x >= _maxX || y >= _maxY || y < 0)
			{
				return null;
			}

			//Log.WriteLine(("Getting " + x + "," +  y));
			return _tiles[GetTileIndex(x, y)];
		}
		

		public class Vein
		{
			public int MinX;
			public int MaxX;
			public int MinY;
			public int MaxY;

			public Vein(string line)
			{
				string[] parts = line.Split(',');

				string first = parts[0].Trim();
				string[] firstParts = first.Split('=');
				if (firstParts[0] == "x")
				{
					MinX = MaxX = Int32.Parse(firstParts[1]);
				} else if (firstParts[0] == "y")
				{
					MinY = MaxY = Int32.Parse(firstParts[1]);
				}

				string second = parts[1].Trim();
				string[] secondParts = second.Split('=');

				string range = secondParts[1].Trim();
				string rangeStart = range.Substring(0, range.IndexOf('.'));
				int pivot = range.LastIndexOf('.');
				string rangeEnd = range.Substring(pivot + 1, range.Length - pivot - 1);
				
				
				if (secondParts[0] == "x")
				{
					
					MinX = Int32.Parse(rangeStart);
					MaxX = Int32.Parse(rangeEnd);
				} else if (secondParts[0] == "y")
				{
					MinY = Int32.Parse(rangeStart);
					MaxY = Int32.Parse(rangeEnd);
				}

			}

			public void DebugLog()
			{
				Log.WriteLine("X: "+ MinX + "," + MaxX + " - Y:" + MinY + "," + MaxY);
			}


		}
		
		
		public class Tile
		{
			
			public int X;
			public int Y;
			public int TileIndex;
			public TileType TileType;

			public Tile Left;
			public Tile Right;
			public Tile Bottom;

			public bool IsFlooded;
			public bool IsSettled;
			public int FloodedOnFrame;
			public Tile FloodedFrom;
			
			public const char Sand = '.';
			public const char Fountain = '+';
			public const char FloodedHoriz = '~';
			public const char FloodedVert = '|';
			public const char Clay = '#';


			

			public char Render()
			{
				if (this.TileType == TileType.Fountain)
				{
					return Fountain;
				}
				if (this.IsFlooded)
				{
					return FloodedHoriz;
				}

				if (this.TileType == TileType.Clay)
				{
					return Clay;
				}

				return Sand;
			}
		}
		
		public enum TileType
		{
			Fountain,
			Sand,
			Clay
		}


		
	}
}