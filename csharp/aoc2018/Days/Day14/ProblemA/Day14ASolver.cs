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
	public class Day14ASolver
	{
		//public const int TargetRecipeCount = 18;
		
		public const int TargetRecipeCount = 919901;

		public const int InitialState = 37;

		public const int ChecksumLen = 10;

		public const int NumElves = 2;

		private static List<int> _gameboard = new List<int>();
		
		private static List<int> _scratch = new List<int>();

		private static StringBuilder _buffer = new StringBuilder();

		private static int[] _elfPositions;

		private static int _currGeneration;
		
		private static int _numRecipesCreated;

		public static void Solve()
		{
			AppendRecipeToGameboard(InitialState);
			_elfPositions = new int[NumElves];
			_elfPositions[0] = 0;
			_elfPositions[1] = 1;
			LogGameboard();
			
			while(_numRecipesCreated < TargetRecipeCount)
			{
				GenerateNewRecipes();
				//LogGameboard();
			}

			int targetLen = TargetRecipeCount;
			int checkSumLength = targetLen + ChecksumLen;
			while(_numRecipesCreated < checkSumLength)
			{
				GenerateNewRecipes();
				//LogGameboard();
			}

			StringBuilder sb = new StringBuilder();
			for (int i = targetLen; i < checkSumLength; i++)
			{
				sb.Append(_gameboard[i]);
			}
			Log.WriteLine("Scores are " + sb.ToString());
		}

		private static void GenerateNewRecipes()
		{
			int newRecipe = 0;
			for (int j = 0; j < _elfPositions.Length; j++)
			{
				int ingredient = _gameboard[_elfPositions[j]];
				newRecipe += ingredient;
			}
				
			AppendRecipeToGameboard(newRecipe);

			for (int j = 0; j < _elfPositions.Length; j++)
			{
				int pos = _elfPositions[j];
				int ingredient = _gameboard[pos];
				int advance = ingredient + 1;
				int newIndex = (pos + advance) % _gameboard.Count;
				_elfPositions[j] = newIndex;
			}

		}
		
		public static void LogGameboard()
		{
			_buffer.Clear();
			for (int i = 0; i < _gameboard.Count; i++)
			{
				int elfIndex = Int32.MinValue;
				for (int j = 0; j < _elfPositions.Length; j++)
				{
					if (_elfPositions[j] == i)
					{
						elfIndex = j;
						break;
					}
				}

				if (elfIndex > Int32.MinValue)
				{
					if (elfIndex == 0)
					{
						//_buffer.Append('(');
						_buffer.Append(_gameboard[i]);
						//_buffer.Append(')');
					} else if (elfIndex == 1)
					{
						//_buffer.Append('[');
						_buffer.Append(_gameboard[i]);
						//_buffer.Append(']');	
					}
					
				}
				else
				{
					//_buffer.Append(' ');
					_buffer.Append(_gameboard[i]);
					//_buffer.Append(' ');
				}
				
				
			}
			Log.WriteLine(_buffer.ToString());
		}

		private static void AppendRecipeToGameboard(int val)
		{
			_scratch.Clear();
			if (val == 0)
			{
				_numRecipesCreated++;
				_scratch.Add(val);
			}
			else
			{
				while (val > 0)
				{
					_numRecipesCreated++;
					_scratch.Add(val % 10);
					val = val / 10;
				}
			}


			_scratch.Reverse();
			_gameboard.AddRange(_scratch);
		}
		
	}
}