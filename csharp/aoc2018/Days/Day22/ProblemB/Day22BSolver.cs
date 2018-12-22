using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day22BSolver
	{


		private static int[] _erosionLevels;
		private static CaveStateNode[] _base;
		private static Dictionary<int, CaveStateNode[]> _allStates = new Dictionary<int, CaveStateNode[]>();
		
		private static int _maxX;
		private static int _maxY;

		private const int _buffer = 100;

		private const int _advancementCost = 1;
		private const int _toolChangeCost = 7;
		
		public static void Solve()
		{
			
			int targetX = 5;
			int targetY = 746;
			int depth = 4002;
			int sumRisk = 0;
			_maxX = (targetX + _buffer);
			_maxY = (targetY + _buffer) ;
			_erosionLevels = new int[_maxX * _maxY];
			_base = new CaveStateNode[_maxX * _maxY];
			//StringBuilder sb = new StringBuilder();
			for (int y = 0; y < _maxY; y++)
			{
				for (int x = 0; x < _maxX; x++)
				{
					int index = Int32.MinValue;
					if (y == 0 && x == 0)
					{
						index = 0;
					} else if (x == targetX && y == targetY)
					{
						index = 0;
					} else if (y == 0)
					{
						index = x * 16807;
					} else if (x == 0)
					{
						index = y * 48271;
					}
					else
					{
						index = _erosionLevels[GetTileIndex(x - 1, y)] * _erosionLevels[GetTileIndex(x, y - 1)];
					}

					int tileIndex = GetTileIndex(x, y);
					int level = (index + depth) % 20183;
					_erosionLevels[tileIndex] = level;

					CaveStateNode state = new CaveStateNode();
					_base[tileIndex] = state;
					
					state.TileX = x;
					state.TileY = y;
					state.TileIndex = tileIndex;
					state.CaveTool = CaveTools.None;
					int typeMod = level % 3;
					
					char c;
					if (y == 0 && x == 0)
					{
						state.TileType = CaveStateNode.Mouth;
					} else if (x == targetX && y == targetY)
					{
						state.TileType = CaveStateNode.Target;
					} else if (typeMod == 0)
					{
						state.TileType = CaveStateNode.Rocky;
					} else if (typeMod == 1)
					{
						state.TileType = CaveStateNode.Wet;
						state.RiskLevel = CaveStateNode.WetRisk;
					}
					else
					{
						state.TileType = CaveStateNode.Narrow;
						state.RiskLevel = CaveStateNode.NarrowRisk;
					}

					if (state.TileX <= targetX && state.TileY <= targetY)
					{
						sumRisk += state.RiskLevel;	
					}
					

				}	
			}
			
			_allStates[CaveTools.None] = new CaveStateNode[_base.Length];
			_allStates[CaveTools.Torch] = new CaveStateNode[_base.Length];
			_allStates[CaveTools.ClimbingGear] = new CaveStateNode[_base.Length];

			for (int y = 0; y < _maxY; y++)
			{
				for (int x = 0; x < _maxX; x++)
				{
					for (int z = 0; z < 3; z++)
					{
						int tileIndex = GetTileIndex(x, y);
						CaveStateNode node = _base[tileIndex];
						if (node.IsValidState(z))
						{
							AddNode(x,y, tileIndex, z, node.TileType);
						}
					}
				}
			}

			List<CaveStateNode> _stateTransitions = new List<CaveStateNode>();
			for (int y = 0; y < _maxY; y++)
			{
				for (int x = 0; x < _maxX; x++)
				{
					_stateTransitions.Clear();
					for (int z = 0; z < 3; z++)
					{
						CaveStateNode node = GetNode(x, y, z);
						if (node != null)
						{
							_stateTransitions.Add(node);
							AddTransitionIfValid(node, x - 1, y, z);
							AddTransitionIfValid(node, x + 1, y, z);
							AddTransitionIfValid(node, x, y +1, z);
							AddTransitionIfValid(node, x, y - 1, z);
						}
					}

					if (_stateTransitions.Count > 1)
					{
						foreach (var a in _stateTransitions)
						{
							foreach (var b in _stateTransitions)
							{
								if (a != b)
								{
									AddTransitionIfValid(a, b.TileX, b.TileY, b.CaveTool);
								}
							}	
						}
					}
				}
			}

			CaveStateNode start = _allStates[CaveTools.Torch][GetTileIndex(0, 0)];
			CaveStateNode end = _allStates[CaveTools.Torch][GetTileIndex(targetX, targetY)];
			
			Log.WriteLine("Start Node");
			Log.WriteLine(start.DebugLog());
			Log.WriteLine(start.DescribeNeighbors());
			Log.WriteLine(Environment.NewLine);
			Log.WriteLine("Target Node");
			Log.WriteLine(end.DebugLog());
			Log.WriteLine(end.DescribeNeighbors());


			int totalCost = 0;
			List<CaveStateNode> bestPath = new List<CaveStateNode>();
			CaveStatePathSolver.RequestPathFromTile(start, end, bestPath, out totalCost);
			Log.WriteLine("Found a path that should take " + totalCost + " minutes");
		}

		private static void AddTransitionIfValid(CaveStateNode node, int p1, int p2, int p3)
		{
			CaveStateNode target = GetNode(p1, p2, p3);
			if (target != null && target != node && target.IsValidState(node.CaveTool))
			{
				if (node.CaveTool != target.CaveTool)
				{
					node.AddTransition(target, _toolChangeCost);	
				}
				else
				{
					node.AddTransition(target, _advancementCost);
				}
				
			}
		}


		public static int GetTileIndex(int x, int y)
		{
			return (_maxX * y) + x;
		}

		public static CaveStateNode GetNode(int x, int y, int state)
		{
			if(x >= _maxX || x < 0 || y >= _maxY || y < 0)
			{
				return null;
			}
			CaveStateNode[] states = _allStates[state];
			//Log.WriteLine(x + "," + y + "," + state);
			return states[GetTileIndex(x, y)];
		}
		
		public static void AddNode(int x, int y, int tileIndex, int state, char type)
		{
			CaveStateNode node = new CaveStateNode();
			node.CaveTool = state;
			node.TileX = x;
			node.TileY = y;
			node.TileIndex = tileIndex;
			node.TileType = type;
			_allStates[state][tileIndex] = node;
		}
	
	}
}

