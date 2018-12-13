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
	public class Day13ASolver
	{
		public static void Solve()
		{
			int sum = 0;
			
			string inputPath = FileUtils.GetProjectFilePath("Days/Day13/ProblemA/input.txt");
			String[] lines = File.ReadAllLines(inputPath);
			MinecartSystem ms = new MinecartSystem(lines);
			ms.Simulate();
		}

		public class MinecartSystem
		{
			private int _turnNumber;
			private MinecartTile[] _tiles;
			private List<Minecart> _carts = new List<Minecart>();
			private Minecart[] _cartLocations;
			private int _rowCount;
			private int _colCount;

			

			public MinecartSystem(string[] lines)
			{
				List<MinecartTile> tiles = new List<MinecartTile>();
				int row = 0;
				int col = 0;
				MinecartTile prevTile = null;
				foreach (var line in lines)
				{
					col = 0;
					foreach(var character in line)
					{
						Minecart m = TryParseMineCart(character, row, col);
						MinecartTile tile = null;
						if(m != null)
						{
							_carts.Add(m);
							tile = m.GenerateImpliedTile();
						}
						else
						{
							tile = ParseTile(character, row, col, prevTile);
						}

						prevTile = tile;
						tiles.Add(tile);
						col++;
					}
					_colCount = col;
					row++;
					if (row >= 2)
					{
						break;
					}
				}
				
				_rowCount = row;
				Log.WriteLine("Size is "+ _rowCount + " x " + _colCount);

				
				_tiles = new MinecartTile[tiles.Count];
				foreach (var tile in tiles)
				{
					tile.TileIndex = GetTileIndex(tile.X, tile.Y);
					_tiles[tile.TileIndex] = tile;
				}
				

				_cartLocations = new Minecart[_tiles.Length];
				
				foreach (var cart in _carts)
				{
					cart.TileIndex = GetTileIndex(cart.X, cart.Y);
					_cartLocations[cart.TileIndex] = cart;
				}

				
				
				RenderGameboard();
			}

			public int GetTileIndex(int x, int y)
			{
				return (_rowCount * x) + y;
			}
			

			private MinecartTile ParseTile(char character, int row, int col, MinecartTile prevTile)
			{
				MinecartTile tile = new MinecartTile();
				tile.X = row;
				tile.Y = col;
				tile.TileType = MinecartTile.CharacterToTileType(character, prevTile);
				Log.WriteLine("Parse " + character + " " + row + "," + col + " ");
				tile.RawCharacter = character;
				return tile;
			}

			public Minecart TryParseMineCart(char tile, int row, int col)
			{
				Minecart res = null;
				if (tile != Minecart.FacingEast && tile != Minecart.FacingWest && tile != Minecart.FacingNorth &&
				    tile != Minecart.FacingSouth)
				{
					return null;
				}
				res = new Minecart();
				res.Orientation = Minecart.CharacterToOrientation(tile);
				res.X = row;
				res.Y = col;
				return res;
			}

			public void Simulate()
			{
				
			}

			public void RenderGameboard()
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < _rowCount; i++)
				{
					for (int j = 0; j < _colCount; j++)
					{
						int tileIndex = GetTileIndex(i, j);
						Minecart mc = _cartLocations[tileIndex];
						if (mc != null)
						{
							sb.Append(mc.Render());
						}
						else
						{
							MinecartTile tile = _tiles[tileIndex];
							sb.Append(tile.Render());
						}
						
					}

					sb.Append(Environment.NewLine);
				}

				Log.WriteLine(sb);
			}
			
		}

		public class MinecartTile
		{
			public int X;
			public int Y;
			public int TileIndex;
			public MinecartTileType TileType;
			public char RawCharacter;
			
			public const char StraightEastWest = '-';
			public const char StraightNorthSouth = '|';
			public const char Intersection = '+';
			public const char CurveNorthToEast = '/';
			public const char CurveEastToSouth = '\\';
			public const char CurveSouthToWest = '/';
			public const char CurveWestToNorth = '\\';



			public static MinecartTileType CharacterToTileType(char character, MinecartTile prevTile)
			{
				switch (character)
				{
					case CurveNorthToEast:
						if (prevTile != null)
						{
							if (prevTile.TileType == MinecartTileType.Intersection ||
							    prevTile.TileType == MinecartTileType.StraightEastWest)
							{
								return MinecartTileType.CurveSouthToWest;
							}
						}
						return MinecartTileType.CurveNorthToEast;
					case CurveEastToSouth:
						if (prevTile != null)
						{
							if (prevTile.TileType == MinecartTileType.Intersection ||
							    prevTile.TileType == MinecartTileType.StraightEastWest)
							{
								return MinecartTileType.CurveEastToSouth;
							}
						}
						return MinecartTileType.CurveWestToNorth;
					case StraightEastWest:
						return MinecartTileType.StraightEastWest;
					case StraightNorthSouth:
						return MinecartTileType.StraightNorthSouth;
					case Intersection:
						return MinecartTileType.Intersection;
					default:
						return MinecartTileType.Empty;
				}
			}

			public char Render()
			{
				switch (TileType)
				{
					case MinecartTileType.StraightEastWest:
						return StraightEastWest;
					case MinecartTileType.StraightNorthSouth:
						return StraightNorthSouth;
					case MinecartTileType.CurveEastToSouth:
						return CurveEastToSouth;
					case MinecartTileType.CurveNorthToEast:
						return CurveNorthToEast;
					case MinecartTileType.CurveSouthToWest:
						return CurveSouthToWest;
					case MinecartTileType.CurveWestToNorth:
						return CurveWestToNorth;
					case MinecartTileType.Intersection:
						return Intersection;
					default:
						return ' ';
				}
			}
		}

		public enum MinecartTileType
		{
			StraightEastWest,
			StraightNorthSouth,
			CurveNorthToEast,
			CurveEastToSouth,
			CurveSouthToWest,
			CurveWestToNorth,
			Intersection,
			Empty
		}

		public class Minecart
		{
			public int X;
			public int Y;
			public int TileIndex;
			public MinecartOrientation Orientation;
			
			public const char FacingWest = '<';
			public const char FacingEast = '>';
			public const char FacingNorth = '^';
			public const char FacingSouth = 'v';

			public static MinecartOrientation CharacterToOrientation(char tile)
			{
				switch (tile)
				{
					case FacingWest:
						return MinecartOrientation.FacingWest;
					case FacingEast:
						return MinecartOrientation.FacingEast;
					case FacingNorth:
						return MinecartOrientation.FacingNorth;
					default:
						return MinecartOrientation.FacingSouth;
				}
			}

			public MinecartTile GenerateImpliedTile()
			{
				MinecartTile tile = new MinecartTile();
				tile.X = this.X;
				tile.Y = this.Y;
				switch (Orientation)
				{
					case MinecartOrientation.FacingWest:
						tile.TileType = MinecartTileType.StraightEastWest;
						break;
					case MinecartOrientation.FacingEast:
						tile.TileType = MinecartTileType.StraightEastWest;
						break;
					case MinecartOrientation.FacingSouth:
						tile.TileType = MinecartTileType.StraightNorthSouth;
						break;
					case MinecartOrientation.FacingNorth:
						tile.TileType = MinecartTileType.StraightNorthSouth;
						break;
				}

				return tile;
			}

			public char Render()
			{
				switch (Orientation)
				{
					case MinecartOrientation.FacingWest:
						return FacingWest;
					case MinecartOrientation.FacingEast:
						return FacingEast;
					case MinecartOrientation.FacingSouth:
						return FacingSouth;
					default:
						return FacingNorth;
				}
			}
		}
		
		public enum MinecartOrientation
		{
			FacingWest,
			FacingEast,
			FacingNorth,
			FacingSouth
		}
	}
}