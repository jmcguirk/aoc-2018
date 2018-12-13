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
	public class Day13BSolver
	{
		public static void Solve()
		{
			int sum = 0;
			
			string inputPath = FileUtils.GetProjectFilePath("Days/Day13/ProblemB/input.txt");
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
			private int _maxX;
			private int _maxY;

			

			public MinecartSystem(string[] lines)
			{
				List<MinecartTile> tiles = new List<MinecartTile>();
				int x = 0;
				int y = 0;
				MinecartTile prevTile = null;
				foreach (var line in lines)
				{
					x = 0;
					foreach(var character in line)
					{
						Minecart m = TryParseMineCart(character, x, y);
						MinecartTile tile = null;
						if(m != null)
						{
							_carts.Add(m);
							tile = m.GenerateImpliedTile();
						}
						else
						{
							tile = ParseTile(character, x, y, prevTile);
						}

						prevTile = tile;
						tiles.Add(tile);
						x++;
					}
					_maxX = x;
					y++;
				}
				
				_maxY = y;

				
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

				
				
				//RenderGameboard();
			}

			public int GetTileIndex(int x, int y)
			{
				return (_maxX * y) + x;
			}
			

			private MinecartTile ParseTile(char character, int x, int y, MinecartTile prevTile)
			{
				MinecartTile tile = new MinecartTile();
				tile.X = x;
				tile.Y = y;
				tile.TileType = MinecartTile.CharacterToTileType(character, prevTile);
				//Log.WriteLine("Parse " + character + " " + row + "," + col + " ");
				tile.RawCharacter = character;
				return tile;
			}

			public Minecart TryParseMineCart(char tile, int x, int y)
			{
				Minecart res = null;
				if (tile != Minecart.FacingEast && tile != Minecart.FacingWest && tile != Minecart.FacingNorth &&
				    tile != Minecart.FacingSouth)
				{
					return null;
				}
				res = new Minecart();
				res.Orientation = Minecart.CharacterToOrientation(tile);
				res.X = x;
				res.Y = y;
				return res;
			}

			public void Simulate()
			{
				RenderGameboard();
				List<Minecart> toRemove = new List<Minecart>();
				while (_carts.Count > 1)
				{
					_carts.Sort(CompareCartsByLocation); // Sort the cars
					toRemove.Clear();
					foreach (var cart in _carts)
					{
						AdvanceCart(cart);
					}
					
					foreach (var cart in _carts)
					{
						if (cart.DidCrash)
						{
							toRemove.Add(cart);	
						}
					}

					foreach (var cart in toRemove)
					{
						if (_cartLocations[cart.TileIndex] == cart)
						{
							_cartLocations[cart.TileIndex] = null;
						}
						_carts.Remove(cart);
					}
					
					//RenderGameboard();		
				}
				Log.WriteLine("Final cart is at " + _carts[0].X + "," + _carts[0].Y);
			}

			private void AdvanceCart(Minecart a)
			{
				if (a.DidCrash)
				{
					return;
				}
				int nextTileX = a.X;
				int nextTileY = a.Y;
				switch (a.Orientation)
				{
					case MinecartOrientation.FacingEast:
						nextTileX++;
						break;
					case MinecartOrientation.FacingWest:
						nextTileX--;
						break;
					case MinecartOrientation.FacingNorth:
						nextTileY--;
						break;
					case MinecartOrientation.FacingSouth:
						nextTileY++;
						break;
				}

				
				int oldTileIndex = a.TileIndex;
				int newTileIndex = GetTileIndex(nextTileX, nextTileY);

				a.X = nextTileX;
				a.Y = nextTileY;
				
				
				Minecart newLocation = _cartLocations[newTileIndex];
				if (newLocation != null && !newLocation.DidCrash)
				{
					newLocation.DidCrash = true;
					a.DidCrash = true;
					return;
				}
				a.TileIndex = newTileIndex;
				_cartLocations[oldTileIndex] = null;
				_cartLocations[newTileIndex] = a;

				
				
				// Adjust orientation for next step
				MinecartTile tile = _tiles[newTileIndex];
				switch (tile.TileType)
				{
					case MinecartTileType.CurveEastToSouth:
						if (a.Orientation == MinecartOrientation.FacingEast)
						{
							a.Orientation = MinecartOrientation.FacingSouth;
						}
						else
						{
							a.Orientation = MinecartOrientation.FacingWest;
						}
						break;
					case MinecartTileType.CurveNorthToEast:
						if (a.Orientation == MinecartOrientation.FacingNorth)
						{
							a.Orientation = MinecartOrientation.FacingEast;
						}
						else
						{
							a.Orientation = MinecartOrientation.FacingSouth;
						}
						break;
					case MinecartTileType.CurveSouthToWest:
						if (a.Orientation == MinecartOrientation.FacingSouth)
						{
							a.Orientation = MinecartOrientation.FacingWest;
						}
						else
						{
							a.Orientation = MinecartOrientation.FacingNorth;
						}
						break;
					case MinecartTileType.CurveWestToNorth:
						if (a.Orientation == MinecartOrientation.FacingWest)
						{
							a.Orientation = MinecartOrientation.FacingNorth;
						}
						else
						{
							a.Orientation = MinecartOrientation.FacingEast;
						}
						break;
					case MinecartTileType.Intersection:

						int turnCount = a.TurnCount;
						if (turnCount == 0) // Left turn
						{
							switch (a.Orientation)
							{
								case MinecartOrientation.FacingNorth:
									a.Orientation = MinecartOrientation.FacingWest;
									break;
								case MinecartOrientation.FacingWest:
									a.Orientation = MinecartOrientation.FacingSouth;
									break;
								case MinecartOrientation.FacingEast:
									a.Orientation = MinecartOrientation.FacingNorth;
									break;
								case MinecartOrientation.FacingSouth:
									a.Orientation = MinecartOrientation.FacingEast;
									break;
							}
						} else if (turnCount == 1) // Straight
						{
							
						} else if (turnCount == 2) // Right turn
						{
							switch (a.Orientation)
							{
								case MinecartOrientation.FacingNorth:
									a.Orientation = MinecartOrientation.FacingEast;
									break;
								case MinecartOrientation.FacingWest:
									a.Orientation = MinecartOrientation.FacingNorth;
									break;
								case MinecartOrientation.FacingEast:
									a.Orientation = MinecartOrientation.FacingSouth;
									break;
								case MinecartOrientation.FacingSouth:
									a.Orientation = MinecartOrientation.FacingWest;
									break;
							}
						} 
						turnCount++;
						a.TurnCount = turnCount % 3;
						break;
				}
			}

			private int CompareCartsByLocation(Minecart a, Minecart b)
			{
				int rowCompare = a.Y.CompareTo(b.Y);
				if (rowCompare != 0)
				{
					return rowCompare;
				}

				return a.X.CompareTo(b.X);
			}

			public void RenderGameboard()
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(Environment.NewLine);
				for (int y = 0; y < _maxY; y++)
				{
					for (int x = 0; x < _maxX; x++)
					{
						int tileIndex = GetTileIndex(x, y);
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
			public bool DidCrash;
			public MinecartOrientation Orientation;
			public int TurnCount;
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