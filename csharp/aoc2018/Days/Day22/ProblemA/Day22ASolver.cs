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
	public class Day22ASolver
	{
		private const char Rocky = '.';
		private const char Wet = '=';
		private const char Narrow = '|';
		private const char Target = 'T';
		private const char Mouth = 'M';

		private const int RockyRisk = 0;
		private const int WetRisk = 1;
		private const int NarrowRisk = 2;

		private static int[] _erosionLevels;
		private static int _maxX;
		private static int _maxY;
		
		public static void Solve()
		{

			int targetX = 5;
			int targetY = 746;
			int depth = 4002;
			int sumRisk = 0;
			_maxX = targetX + 1;
			_maxY = targetY + 1;
			_erosionLevels = new int[_maxX * _maxY];

			StringBuilder sb = new StringBuilder();
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

					
					int level = (index + depth) % 20183;
					_erosionLevels[GetTileIndex(x, y)] = level;

					int typeMod = level % 3;
					
					char c;
					if (y == 0 && x == 0)
					{
						c = Mouth;
					} else if (x == targetX && y == targetY)
					{
						c = Target;
					} else if (typeMod == 0)
					{
						c = Rocky;
					} else if (typeMod == 1)
					{
						c = Wet;
						sumRisk += WetRisk;
					}
					else
					{
						c = Narrow;
						sumRisk += NarrowRisk;
					}

					sb.Append(c);
				}	
				sb.Append(Environment.NewLine);
			}
			Log.WriteLine(sb.ToString());
			Log.WriteLine(Environment.NewLine);
			Log.WriteLine("Total risk is " + sumRisk);
		}
		
		public static int GetTileIndex(int x, int y)
		{
			return (_maxX * y) + x;
		}

		
	
	}
}

